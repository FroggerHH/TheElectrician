using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Settings.Basic;

public sealed record StorageSettings(
    Type type,
    int startLevel,
    int maxLevel,
    int maxConnections,
    float powerLoss,
    int conductivity,
    float maxDistance,
    int maxWeight,
    int maxDifferentItemsCount,
    int powerCapacity,
    int otherCapacity,
    string[] allowedKeys = null)
    : ILevelableSettings, IWireConnectableSettings,
        IItemPipeConnectableSettings, IStorageSettings
{
    public Type type { get; } = type;
    public int startLevel { get; } = startLevel;
    public int maxLevel { get; } = maxLevel;
    public int maxConnections { get; } = maxConnections;
    public float powerLoss { get; } = powerLoss;
    public int conductivity { get; } = conductivity;
    public float maxDistance { get; } = maxDistance;
    public int maxWeight { get; } = maxWeight;
    public int maxDifferentItemsCount { get; } = maxDifferentItemsCount;
    public int powerCapacity { get; } = powerCapacity;
    public int otherCapacity { get; } = otherCapacity;
    public string[] allowedKeys { get; } = allowedKeys ?? [Consts.storagePowerKey];
}