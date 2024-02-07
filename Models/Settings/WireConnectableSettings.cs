namespace TheElectrician.Models.Settings;

public class WireConnectableSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float conductivity,
    float powerLoss,
    int maxConnections)
    : LevelableSettings(type, startLevel, maxLevel)
{
    public readonly float conductivity = conductivity;
    public readonly float powerLoss = powerLoss;
    public int maxConnections = maxConnections;

    public override string ToString()
    {
        return $"{base.ToString()} "
               + $"maxLevel={maxLevel} "
               + $"startLevel={startLevel} "
               + $"conductivity={conductivity} "
               + $"powerLoss={powerLoss} "
               + $"maxConnections={maxConnections}";
    }
}