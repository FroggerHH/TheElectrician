using TheElectrician.Models;
using TheElectrician.Models.Settings;

namespace TheElectrician.Objects;

public class Generator : Storage, IGenerator
{
    public Generator(ZDO ZDO) : base(ZDO) { }

    public override void InitSettings(ElectricObjectSettings settings)
    {
        base.InitSettings(settings);
        if (settings is not GeneratorSettings generatorSettings)
        {
            DebugError("InitSettings: Generator settings is not GeneratorSettings");
            return;
        }

        SetPowerPerTick(generatorSettings.powerPerTick);
        SetFuelItem(generatorSettings.fuelItem);
        SetFuelPerTick(generatorSettings.fuelPerTick);
        SetMaxFuel(generatorSettings.maxFuel);
    }

    public float GetPowerPerTick()
    {
        var powerPerTick = GetZDO().GetFloat(Consts.powerPerTickKey, -1);
        if (powerPerTick == -1) DebugWarning($"Power per tick of the generator {GetZDO()} has not defined");
        return powerPerTick;
    }

    public override void Update()
    {
        base.Update();
        if (!HasFuel()) return;

        var powerPerTick = GetPowerPerTick();
        if (!CanAdd(powerPerTick))
        {
            Debug($"Generator {GetZDO()} is full of power");
            return;
        }
 
        var fuelPerTick = GetFuelPerTick();
        if (!RemoveFuel(fuelPerTick))
        {
            DebugError($"Generator has {GetFuelStored()} fuel but can't remove {fuelPerTick} fuel");
            return;
        }

        Add(Consts.storagePowerKey, powerPerTick);
        Debug($"Generator {GetZDO()} produced {powerPerTick} power");
    }

    public void SetPowerPerTick(float power) { GetZDO().Set(Consts.powerPerTickKey, power); }

    public void SetActive(bool active) { GetZDO().Set(ZDOVars.s_enabled, active); }

    public bool IsActive() { return GetZDO().GetBool(ZDOVars.s_enabled, true); }

    public bool HasFuel()
    {
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
            GetZDO().Set(Consts.fuelItemKey, "Coal");
            DebugWarning($"Fuel of the generator {GetZDO()} has not defined, set to default: Coal");
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
            GetZDO().Set(Consts.maxFuelKey, 100);
            DebugWarning($"Max fuel of the generator {GetZDO()} has not defined, set to default: 100");
        }

        return Min(GetCapacity(), maxFuel);
    }

    public void SetMaxFuel(int amount) { GetZDO().Set(Consts.maxFuelKey, Min(GetCapacity(), amount)); }

    public float GetFuelPerTick()
    {
        var fuelPerTick = GetZDO().GetFloat(Consts.fuelPerTickKey, -1);
        if (fuelPerTick == -1)
        {
            GetZDO().Set(Consts.fuelPerTickKey, 1f);
            fuelPerTick = 1f;
            DebugWarning($"Fuel per tick of the generator {GetZDO()} has not defined, set to default: 1");
        }

        return fuelPerTick;
    }

    public void SetFuelPerTick(float amount) { GetZDO().Set(Consts.fuelPerTickKey, amount); }
}