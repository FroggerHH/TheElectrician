using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Settings.Basic;

public sealed record FurnaceSettings(
    Type type,
    int startLevel,
    int maxLevel,
    int powerCapacity,
    int otherCapacity,
    int maxConnections,
    float powerLoss,
    int conductivity,
    float maxDistance,
    string[] allowedKeys = null)
    : ILevelableSettings, IStorageSettings, IWireConnectableSettings,
        IItemPipeConnectableSettings
{
    public Type type { get; } = type;
    public int startLevel { get; } = startLevel;
    public int maxLevel { get; } = maxLevel;
    public int powerCapacity { get; } = powerCapacity;
    public int otherCapacity { get; } = otherCapacity;
    public string[] allowedKeys { get; } = allowedKeys;
    public int maxConnections { get; } = maxConnections;
    public float powerLoss { get; } = powerLoss;
    public int conductivity { get; } = conductivity;
    public float maxDistance { get; } = maxDistance;
}