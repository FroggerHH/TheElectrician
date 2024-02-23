namespace TheElectrician.Settings;

public class ItemPipeSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float conductivity,
    int maxConnections,
    int maxWeight,
    int maxDifferentItemsCount)
    : ItemPipeConnectableSettings(type, startLevel, maxLevel, conductivity, maxConnections, maxWeight, maxDifferentItemsCount)
{
}