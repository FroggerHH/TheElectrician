using TheElectrician.Models.Settings;
using TheElectrician.Systems.PowerFlow;

namespace TheElectrician.Objects.Consumers.Furnace;

public class Furnace : Storage, IFurnace
{
    private float currentPower;
    private List<FurnaceRecipe> cachedRecipes;
    private string[] cachedAllowedKeys;
    private FurnaceSettings furnaceSettings;
    private FurnaceState state;
    private FurnaceRecipe currentRecipe;
    private int ticksElapsed;

    public UnityEvent onProgressStarted { get; private set; }
    public UnityEvent onProgressChanged { get; private set; }
    public UnityEvent onProgressCompleted { get; private set; }
    public FurnaceState GetState() => state;

    private void SetState(FurnaceState newState) => state = newState;

    public bool IsInWorkingState() => state == FurnaceState.Working;

    public (int start, int end) GetProgress()
    {
        if (currentRecipe is null) return (0, 0);
        return (ticksElapsed, currentRecipe.CalculateTicks(GetLevel()));
    }

    public FurnaceRecipe GetCurrentRecipe() => currentRecipe;

    public override void InitSettings(ElectricObjectSettings sett)
    {
        base.InitSettings(sett);
        furnaceSettings = GetSettings<FurnaceSettings>();
        if (furnaceSettings is null)
            DebugError($"Furnace.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not FurnaceSettings");
    }

    public override void InitData()
    {
        base.InitData();
        var level = GetLevel();
        cachedRecipes = FurnaceRecipe.GetAllRecipes(level);
        cachedAllowedKeys = cachedRecipes.Select(x => x.output).Union(cachedRecipes.Select(x => x.input)).ToArray();

        onProgressChanged = new();
        onProgressCompleted = new();
        onProgressStarted = new();
    }

    public float GetPossiblePower() => currentPower;

    public override void Update()
    {
        base.Update();
        if (!IsValid()) return;
        currentPower = PowerFlow.GetPowerSystem(this)?.GetPossiblePowerInElement(this) ?? 0;
        if (IsInWorkingState())
        {
            UpdateWorkingState();
            return;
        }

        if (IsFull()) return;
        currentRecipe = FindRecipe();
        if (currentRecipe is not null && HaveEnoughPower())
        {
            onProgressStarted?.Invoke();
            SetState(FurnaceState.Working);
            // ReSharper disable once RedundantJumpStatement Keep this here, because of furnace state change
            return;
        }
    }

    private void UpdateWorkingState()
    {
        if (!CanProduceRecipe(currentRecipe, false))
        {
            SetState(FurnaceState.Idle);
            return;
        }

        if (!HaveEnoughPower()) return;

        if (ticksElapsed < currentRecipe.CalculateTicks(GetLevel()))
        {
            AddProgress();
            return;
        }

        onProgressChanged?.Invoke();
        Add(currentRecipe.output, currentRecipe.outputCount);
        Remove(currentRecipe.input, currentRecipe.inputCount);
        PowerFlow.ConsumePower(this, GetPowerNeeded());
        // SetState(FurnaceState.Idle);
        ticksElapsed = 0;
        onProgressCompleted?.Invoke();
    }

    private bool CanProduceRecipe(FurnaceRecipe recipe, bool checkPower = true)
    {
        var enoughPower = !checkPower || HaveEnoughPower(recipe);
        var canAdd = CanAdd(recipe.output, recipe.outputCount);
        var canAddRemove = CanRemove(recipe.input, recipe.inputCount);

        return canAdd && canAddRemove && enoughPower;
    }

    public bool HaveEnoughPower() => HaveEnoughPower(currentRecipe);

    public bool HaveEnoughPower(FurnaceRecipe recipe) => currentPower >= GetPowerNeeded(recipe);

    private float GetPowerNeeded(FurnaceRecipe recipe) { return recipe?.CalculatePower(GetLevel()) ?? 0.001f; }
    private float GetPowerNeeded() => GetPowerNeeded(currentRecipe);


    private void AddProgress()
    {
        ticksElapsed++;
        onProgressChanged?.Invoke();
    }

    private FurnaceRecipe FindRecipe() =>
        cachedRecipes.FirstOrDefault(x => CanProduceRecipe(x)) ??
        cachedRecipes.FirstOrDefault(x => CanProduceRecipe(x, false));

    public override string[] GetAllowedKeys() => cachedAllowedKeys;
}