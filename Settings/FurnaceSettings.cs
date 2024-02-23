namespace TheElectrician.Settings;

public class FurnaceSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float powerLoss,
    int maxConnections,
    float maxDistance,
    int powerCapacity,
    int otherCapacity)
    : ConsumerSettings(type, startLevel, maxLevel, int.MaxValue, powerLoss, maxConnections, maxDistance, powerCapacity, otherCapacity);