namespace TheElectrician.Settings;

public class ConsumerSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float conductivity,
    float powerLoss,
    int maxConnections,
    float maxDistance,
    int powerCapacity,
    int otherCapacity)
    : StorageSettings(type, startLevel, maxLevel, conductivity, powerLoss, maxConnections, maxDistance, powerCapacity, otherCapacity,
        [Consts.storagePowerKey]);