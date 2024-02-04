namespace TheElectrician.Models.Settings;

public class GeneratorSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float conductivity,
    int powerLoss,
    int maxConnections,
    int capacity,
    float powerPerTick,
    string fuelItem,
    float fuelPerTick,
    int maxFuel)
    : StorageSettings(type, startLevel, maxLevel, conductivity, powerLoss, maxConnections, capacity,
        [fuelItem, Consts.storagePowerKey])
{
    public readonly string fuelItem = fuelItem;
    public readonly float fuelPerTick = fuelPerTick;
    public readonly int maxFuel = maxFuel;
    public readonly float powerPerTick = powerPerTick;

    public override string ToString()
    {
        return $"{base.ToString()} "
               + $"fuelItem={fuelItem} "
               + $"fuelPerTick={fuelPerTick} "
               + $"maxFuel={maxFuel} "
               + $"powerPerTick={powerPerTick} ";
    }
}