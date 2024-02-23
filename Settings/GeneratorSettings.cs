namespace TheElectrician.Settings;

public class GeneratorSettings(
    Type type,
    int startLevel,
    int maxLevel,
    int powerLoss,
    int maxConnections,
    float maxDistance,
    int powerCapacity,
    int otherCapacity,
    float powerPerTick,
    string fuelItem,
    float fuelPerTick,
    int maxFuel)
    : StorageSettings(type, startLevel, maxLevel, int.MaxValue, powerLoss, maxConnections, maxDistance, powerCapacity, otherCapacity,
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