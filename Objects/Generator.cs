using TheElectrician.Models.Settings;

namespace TheElectrician.Objects;

public class Generator : Storage, IGenerator
{
    private GeneratorSettings generatorSettings;

    public override void InitSettings(ElectricObjectSettings settings)
    {
        base.InitSettings(settings);
        generatorSettings = GetSettings<GeneratorSettings>();
        if (generatorSettings is null)
            DebugError($"Generator.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not GeneratorSettings");
    }

    public float GetPowerPerTick() => generatorSettings.powerPerTick;

    public override void Update()
    {
        base.Update();
        if (!HasFuel()) return;

        var powerPerTick = GetPowerPerTick();
        var fuelPerTick = GetFuelPerTick();

        if (!CanAdd(Consts.storagePowerKey, powerPerTick))
            return;

        if (!RemoveFuel(fuelPerTick))
        {
            DebugError($"Generator has {GetFuelStored()} fuel but can't remove {fuelPerTick} fuel");
            return;
        }

        Add(Consts.storagePowerKey, powerPerTick);
    }

    public void SetActive(bool active) { GetZDO().Set(ZDOVars.s_enabled, active); }

    public bool IsActive() { return GetZDO()?.GetBool(ZDOVars.s_enabled, true) ?? true; }

    public bool HasFuel() { return GetFuelStored() >= GetFuelPerTick(); }

    public bool AddFuel(float amount) { return Add(GetFuelItem(), amount); }

    public bool RemoveFuel(float amount) { return Remove(GetFuelItem(), amount); }

    public float GetFuelStored() { return Count(GetFuelItem()); }

    public string GetFuelItem() => generatorSettings.fuelItem;

    public void SetFuel(float amount) { SetStored(GetFuelItem(), amount); }

    public int GetMaxFuel() => generatorSettings.maxFuel;

    public float GetFuelPerTick() => generatorSettings.fuelPerTick;

    public override string ToString()
    {
        if (!IsValid()) return "Uninitialized Generator";
        return $"Generator {GetId()}, zdo: {GetZDO()}";
    }
}