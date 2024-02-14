namespace TheElectrician.Models.Settings;

public class FurnaceSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float powerLoss,
    int maxConnections,
    int capacity)
    : StorageSettings(type, startLevel, maxLevel, int.MaxValue, powerLoss, maxConnections, capacity);