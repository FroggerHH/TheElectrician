using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Settings.Basic;

public sealed record ItemPipeSettings(
    Type type,
    int startLevel,
    int maxLevel,
    int maxConnections,
    int maxWeight,
    int maxDifferentItemsCount)
    : ILevelableSettings, IItemPipeSettings
{
    public Type type { get; } = type;
    public int startLevel { get; } = startLevel;
    public int maxLevel { get; } = maxLevel;
    public int maxConnections { get; } = maxConnections;
    public int maxWeight { get; } = maxWeight;
    public int maxDifferentItemsCount { get; } = maxDifferentItemsCount;
}