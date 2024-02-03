namespace TheElectrician.Models.Settings;

public class StorageSettings : WireConnectableSettings
{
    public readonly int capacity;
    public readonly bool storeOnlyPower;

    public StorageSettings(
        Type type, int startLevel, int maxLevel, float conductivity, float powerLoss, int maxConnections,
        int capacity, bool storeOnlyPower = true)
        : base(type, startLevel, maxLevel, conductivity, powerLoss, maxConnections)
    {
        this.capacity = capacity;
        this.storeOnlyPower = storeOnlyPower;
    }

    public override string ToString() { return $"{base.ToString()} capacity={capacity}"; }
}