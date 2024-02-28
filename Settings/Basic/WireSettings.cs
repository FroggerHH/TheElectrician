using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Settings.Basic;

public sealed record WireSettings(
    Type type,
    int startLevel,
    int maxLevel,
    int maxConnections,
    float powerLoss,
    int conductivity,
    float maxDistance)
    : ILevelableSettings, IWireSettings
{
    public Type type { get; } = type;
    public int startLevel { get; } = startLevel;
    public int maxLevel { get; } = maxLevel;
    public int maxConnections { get; } = maxConnections;
    public float powerLoss { get; } = powerLoss;
    public int conductivity { get; } = conductivity;
    public float maxDistance { get; } = maxDistance;
}