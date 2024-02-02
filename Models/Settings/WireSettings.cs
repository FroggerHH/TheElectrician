namespace TheElectrician.Models.Settings;

public class WireSettings : WireConnectableSettings
{
    public WireSettings(Type type, int startLevel, int maxLevel, float conductivity, float powerLossPercentage, int maxConnections) :
        base(type, startLevel, maxLevel, conductivity, powerLossPercentage, maxConnections)
    {
    }
}