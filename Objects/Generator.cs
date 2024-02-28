using TheElectrician.Settings;
using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Objects;

public class Generator : Storage, IGenerator
{
    private IGeneratorSettings generatorSettings;

    public override void InitSettings(IElectricObjectSettings settings)
    {
        base.InitSettings(settings);
        generatorSettings = GetSettings<IGeneratorSettings>();
        if (generatorSettings is null)
            DebugError($"{GetType().Name}.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not IGeneratorSettings");
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
}