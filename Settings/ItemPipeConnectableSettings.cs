namespace TheElectrician.Settings;

public class ItemPipeConnectableSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float conductivity,
    int maxConnections,
    int maxWeight,
    int maxDifferentItemsCount)
    : PipeConnectableSettings(type, startLevel, maxLevel, conductivity, maxConnections, 0)
{
    public int maxWeight = maxWeight;
    public int maxDifferentItemsCount = maxDifferentItemsCount;
}