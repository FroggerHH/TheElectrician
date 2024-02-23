namespace TheElectrician.Settings;

public class PipeConnectableSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float conductivity,
    int maxConnections,
    float maxDistance)
    : LevelableSettings(type, startLevel, maxLevel)
{
    public readonly float conductivity = conductivity;
    public readonly int maxConnections = maxConnections;
    public readonly float maxDistance = maxDistance;

    public override string ToString()
    {
        return $"{base.ToString()} "
               + $"maxLevel={maxLevel} "
               + $"startLevel={startLevel} "
               + $"conductivity={conductivity} "
               + $"maxConnections={maxConnections} "
               + $"maxDistance={maxDistance}";
    }
}