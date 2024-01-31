using TheElectrician.Models;
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

    public override void InitData()
    {
        base.InitData();
        GetFuelItem();
        GetPowerPerTick();
        GetFuelPerTick();
        GetMaxFuel();
    }

    public float GetPowerPerTick()
    {
        var powerPerTick = GetZDO().GetFloat(Consts.powerPerTickKey, -1);
        if (powerPerTick == -1)
        {
            powerPerTick = generatorSettings.powerPerTick;
            SetPowerPerTick(powerPerTick);
        }

        return powerPerTick;
    }

    public override void Update()
    {
        base.Update();
        if (!HasFuel()) return;
        Debug($"Generator update 1");

        var powerPerTick = GetPowerPerTick();
        var fuelPerTick = GetFuelPerTick();
        if (!CanAdd(powerPerTick))
        {
            Debug($"Generator {GetZDO()} is full of power");
            return;
        }

        if (!RemoveFuel(fuelPerTick))
        {
            DebugError($"Generator has {GetFuelStored()} fuel but can't remove {fuelPerTick} fuel");
            return;
        }

        Add(Consts.storagePowerKey, powerPerTick);
    }

    public void SetPowerPerTick(float amount) { GetZDO().Set(Consts.powerPerTickKey, amount); }

    public void SetActive(bool active) { GetZDO().Set(ZDOVars.s_enabled, active); }

    public bool IsActive() { return GetZDO()?.GetBool(ZDOVars.s_enabled, true) ?? true; }

    public bool HasFuel()
    {
        Debug($"fuel stored: {GetFuelStored()}, fuel per tick: {GetFuelPerTick()}");
        return GetFuelStored() >= GetFuelPerTick();
    }

    public bool AddFuel(float amount) { return Add(GetFuelItem(), amount); }

    public bool RemoveFuel(float amount) { return Remove(GetFuelItem(), amount); }

    public float GetFuelStored() { return Count(GetFuelItem()); }

    public string GetFuelItem()
    {
        var fuel = GetZDO().GetString(Consts.fuelItemKey, "None");
        if (fuel == "None")
        {
            if (generatorSettings is null)
            {
                DebugError($"Generator {GetZDO()} has no settings");
            } else
            {
                fuel = generatorSettings.fuelItem;
                SetFuelItem(fuel);
            }
        }

        return fuel;
    }

    public void SetFuelItem(string item)
    {
        if (!item.IsGood())
        {
            DebugError($"Generator {GetZDO()} has bad fuel item: {item}, set to default: Coal");
            GetZDO().Set(Consts.fuelItemKey, "Coal");
            return;
        }

        if (ZNetScene.instance && !ZNetScene.instance.GetPrefab(item))
        {
            DebugError($"Item {item} of the generator {GetZDO()} not found, set to default: Coal");
            GetZDO().Set(Consts.fuelItemKey, "Coal");
            return;
        }

        GetZDO().Set(Consts.fuelItemKey, item);
    }


    public void SetFuel(float amount) { SetStored(GetFuelItem(), amount); }

    public int GetMaxFuel()
    {
        var maxFuel = GetZDO().GetInt(Consts.maxFuelKey, -1);
        if (maxFuel == -1)
        {
            maxFuel = generatorSettings.maxFuel;
            SetMaxFuel(maxFuel);
        }

        return Min(GetCapacity(), maxFuel);
    }

    public void SetMaxFuel(int amount) { GetZDO().Set(Consts.maxFuelKey, Min(GetCapacity(), amount)); }

    public float GetFuelPerTick()
    {
        var fuelPerTick = GetZDO().GetFloat(Consts.fuelPerTickKey, -1);
        if (fuelPerTick == -1)
        {
            fuelPerTick = generatorSettings.fuelPerTick;
            SetFuelPerTick(fuelPerTick);
        }

        return fuelPerTick;
    }

    public void SetFuelPerTick(float amount) { GetZDO().Set(Consts.fuelPerTickKey, amount); }

    public override string ToString()
    {
        // string basic = base.ToString();
        // return $"Generator {GetId()}, active: {IsActive()}, {basic}";
        if (!IsValid()) return "Uninitialized Generator";
        return $"Generator {GetId()}, zdo: {GetZDO()}";
    }
}