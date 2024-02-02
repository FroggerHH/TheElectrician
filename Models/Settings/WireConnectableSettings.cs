namespace TheElectrician.Models.Settings;

public class WireConnectableSettings : LevelableSettings
{
    public readonly float conductivity;
    public readonly float powerLossPercentage;
    public int maxConnections;

    public WireConnectableSettings(Type type, int startLevel, int maxLevel, float conductivity,
        float powerLossPercentage, int maxConnections) : base(type, startLevel, maxLevel)
    {
        this.conductivity = conductivity;
        this.powerLossPercentage = powerLossPercentage;
        this.maxConnections = maxConnections;
    }

    public override string ToString()
    {
        return $"{base.ToString()} "
               + $"maxLevel={maxLevel} "
               + $"startLevel={startLevel} "
               + $"conductivity={conductivity} "
               + $"powerLossPercentage={powerLossPercentage} "
               + $"maxConnections={maxConnections}";
    }
}