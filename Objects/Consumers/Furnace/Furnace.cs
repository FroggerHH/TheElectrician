using TheElectrician.Models;
using TheElectrician.Models.Settings;
using TheElectrician.Systems.PowerFlow;
using UnityEngine.Events;

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

    public UnityEvent onProgressAdded { get; private set; }
    public UnityEvent onProgressCompleted { get; private set; }
    public FurnaceState GetState() => state;

    private void SetState(FurnaceState newState) => state = newState;

    public bool IsInWorkingState() => state == FurnaceState.Working;

    public RangeInt GetProgress()
    {
        if (currentRecipe is null) return new RangeInt(0, 0);
        return new RangeInt(ticksElapsed, currentRecipe.CalculateTicks(GetLevel()));
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

        onProgressAdded = new();
        onProgressCompleted = new();
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
        if (currentRecipe is not null)
        {
            SetState(FurnaceState.Working);
            return;
        }
    }

    private void UpdateWorkingState()
    {
        if (!CanProduceRecipe(currentRecipe))
        {
            SetState(FurnaceState.Idle);
            return;
        }

        if (ticksElapsed < currentRecipe.CalculateTicks(GetLevel()))
        {
            AddProgress();
            return;
        }

        Add(currentRecipe.output, currentRecipe.outputCount);
        Remove(currentRecipe.input, currentRecipe.inputCount);
        SetState(FurnaceState.Idle);
        ticksElapsed = 0;
        onProgressCompleted?.Invoke();
    }

    private bool CanProduceRecipe(FurnaceRecipe recipe)
    {
        var enoughPower = HaveEnoughPower(recipe);
        var canAdd = CanAdd(recipe.output, recipe.outputCount);
        var canAddRemove = CanRemove(recipe.input, recipe.inputCount);

        // Debug($"CanProduceRecipe ({recipe.input}->{recipe.output}) {canAdd} {canAddRemove} {enoughPower}");
        return canAdd && canAddRemove && enoughPower;
    }

    public bool HaveEnoughPower() => HaveEnoughPower(currentRecipe);

    public bool HaveEnoughPower(FurnaceRecipe recipe)
    {
        return currentPower >= (recipe?.CalculatePower(GetLevel()) ?? 0.001);
    }


    private void AddProgress()
    {
        ticksElapsed++;
        onProgressAdded?.Invoke();
    }

    private FurnaceRecipe FindRecipe() => cachedRecipes.FirstOrDefault(x => CanProduceRecipe(x));

    public override string[] GetAllowedKeys() => cachedAllowedKeys;
}