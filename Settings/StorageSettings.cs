namespace TheElectrician.Settings;

public class StorageSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float conductivity,
    float powerLoss,
    int maxConnections,
    float maxDistance,
    int powerCapacity,
    int otherCapacity,
    string[] allowedKeys = null)
    : WireConnectableSettings(type, startLevel, maxLevel, conductivity, powerLoss, maxConnections, maxDistance)
{
    public int otherCapacity = otherCapacity;
    public readonly int powerCapacity = powerCapacity;
    public readonly string[] allowedKeys = allowedKeys ?? [Consts.storagePowerKey];

    public override string ToString() => $"{base.ToString()} powerCapacity={powerCapacity} otherCapacity={otherCapacity} allowedKeys={allowedKeys}";
}