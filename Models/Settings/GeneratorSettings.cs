namespace TheElectrician.Models.Settings;

public class GeneratorSettings : StorageSettings
{
    public readonly string fuelItem;
    public readonly float fuelPerTick;
    public readonly int maxFuel;
    public readonly float powerPerTick;

    public GeneratorSettings(Type type, int capacity,
        float powerPerTick, string fuelItem, float fuelPerTick, int maxFuel)
        : base(type, capacity)
    {
        this.powerPerTick = powerPerTick;
        this.fuelItem = fuelItem;
        this.fuelPerTick = fuelPerTick;
        this.maxFuel = maxFuel;
    }
}