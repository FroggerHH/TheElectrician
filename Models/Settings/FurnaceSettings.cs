namespace TheElectrician.Models.Settings;

public class FurnaceSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float conductivity,
    float powerLoss,
    int maxConnections,
    int capacity)
    : StorageSettings(type, startLevel, maxLevel, conductivity, powerLoss, maxConnections, capacity);