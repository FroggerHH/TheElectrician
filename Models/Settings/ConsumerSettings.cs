namespace TheElectrician.Models.Settings;

public class ConsumerSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float conductivity,
    float powerLoss,
    int maxConnections,
    int powerCapacity,
    int otherCapacity)
    : StorageSettings(type, startLevel, maxLevel, conductivity, powerLoss, maxConnections, powerCapacity, otherCapacity,
        [Consts.storagePowerKey]);