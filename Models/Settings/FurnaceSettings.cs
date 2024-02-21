namespace TheElectrician.Models.Settings;

public class FurnaceSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float powerLoss,
    int maxConnections,
    int powerCapacity,
    int otherCapacity)
    : ConsumerSettings(type, startLevel, maxLevel, int.MaxValue, powerLoss, maxConnections, powerCapacity, otherCapacity);