namespace TheElectrician.Models.Settings;

public class WireConnectableSettings : LevelableSettings
{
    public readonly float conductivity;
    public readonly float powerLoss;
    public int maxConnections;

    public WireConnectableSettings(Type type, int startLevel, int maxLevel, float conductivity,
        float powerLoss, int maxConnections) : base(type, startLevel, maxLevel)
    {
        this.conductivity = conductivity;
        this.powerLoss = powerLoss;
        this.maxConnections = maxConnections;
    }

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