namespace TheElectrician.Models.Settings;

public class StorageSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float conductivity,
    float powerLoss,
    int maxConnections,
    int capacity,
    string[] allowedKeys = null)
    : WireConnectableSettings(type, startLevel, maxLevel, conductivity, powerLoss, maxConnections)
{
    public readonly int capacity = capacity;
    public readonly string[] allowedKeys = allowedKeys ?? [Consts.storagePowerKey];

    public override string ToString() => $"{base.ToString()} capacity={capacity} allowedKeys={allowedKeys}";
}