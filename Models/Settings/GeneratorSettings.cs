namespace TheElectrician.Models.Settings;

public class GeneratorSettings : StorageSettings
{
    public readonly string fuelItem;
    public readonly float fuelPerTick;
    public readonly int maxFuel;
    public readonly float powerPerTick;

    public GeneratorSettings(Type type, int startLevel, int maxLevel, int powerLossPercentage, float conductivity,
        int maxConnections,
        int capacity, float powerPerTick, string fuelItem, float fuelPerTick, int maxFuel, bool storeOnlyPower = false)
        : base(type, startLevel, maxLevel, conductivity, powerLossPercentage, maxConnections, capacity, storeOnlyPower)
    {
        this.powerPerTick = powerPerTick;
        this.fuelItem = fuelItem;
        this.fuelPerTick = fuelPerTick;
        this.maxFuel = maxFuel;
    }

    public override string ToString()
    {
        return $"{base.ToString()} "
               + $"fuelItem={fuelItem} "
               + $"fuelPerTick={fuelPerTick} "
               + $"maxFuel={maxFuel} "
               + $"powerPerTick={powerPerTick} ";
    }
}