using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Settings.Basic;

public sealed record GeneratorSettings(
    Type type,
    int startLevel,
    int maxLevel,
    int maxConnections,
    float powerLoss,
    int conductivity,
    float maxDistance,
    int powerCapacity,
    int otherCapacity,
    string fuelItem,
    float fuelPerTick,
    int maxFuel,
    float powerPerTick,
    string[] allowedKeys = null)
    : ILevelableSettings, IWireConnectableSettings,
        IItemPipeConnectableSettings, IGeneratorSettings
{
    public Type type { get; } = type;
    public int startLevel { get; } = startLevel;
    public int maxLevel { get; } = maxLevel;
    public int maxConnections { get; } = maxConnections;
    public float powerLoss { get; } = powerLoss;
    public int conductivity { get; } = conductivity;
    public float maxDistance { get; } = maxDistance;
    public int powerCapacity { get; } = powerCapacity;
    public int otherCapacity { get; } = otherCapacity;
    public string[] allowedKeys { get; } = allowedKeys ?? [Consts.storagePowerKey, fuelItem];
    public string fuelItem { get; } = fuelItem;
    public float fuelPerTick { get; } = fuelPerTick;
    public int maxFuel { get; } = maxFuel;
    public float powerPerTick { get; } = powerPerTick;
}