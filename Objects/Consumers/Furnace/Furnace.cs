using TheElectrician.Models;
using TheElectrician.Models.Settings;
using UnityEngine.Events;

namespace TheElectrician.Objects.Consumers.Furnace;

public class Furnace : Storage, IFurnace
{
    private List<FurnaceRecipe> cachedRecipes;
    private string[] cachedAllowedKeys;
    private FurnaceSettings furnaceSettings;
    private FurnaceState state;
    private FurnaceRecipe currentRecipe;
    private int ticksElapsed;

    public UnityEvent onProgressAdded { get; private set; }
    public UnityEvent onProgressCompleted { get; private set; }
    public FurnaceState GetState() => state;

    public bool IsInWorkingState() => state == FurnaceState.Working;

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
        cachedRecipes = FurnaceRecipe.GetAllRecipes(GetLevel());
        cachedAllowedKeys = cachedRecipes.Select(x => x.output).Union(cachedRecipes.Select(x => x.input)).ToArray();
        
        onProgressAdded = new();
        onProgressCompleted = new();
    }

    public override void Update()
    {
        base.Update();
        if (!IsValid()) return;
        if (!IsInWorkingState())
        {
            UpdateWorkingState();
            return;
        }

        if (IsFull()) return;

        currentRecipe = FindRecipe();
    }

    private void UpdateWorkingState()
    {
        if (ticksElapsed < currentRecipe.CalculateTicks(GetLevel()))
        {
            AddProgress();
            return;
        }
        //ProcessRecipe(currentRecipe);
    }

    private void AddProgress()
    {
        ticksElapsed++;
        onProgressAdded?.Invoke();
    }

    private FurnaceRecipe FindRecipe()
    {
        foreach (var recipe in cachedRecipes)
        {
            if (!CanAdd(recipe.output, recipe.outputCount)) continue;
            if (!CanRemove(recipe.input, recipe.inputCount)) continue;

            return recipe;
        }

        return null;
    }

    public override bool CanAdd(string key, float amount) { return base.CanAdd(key, amount) && !IsInWorkingState(); }

    public override string[] GetAllowedKeys() => cachedAllowedKeys;
}