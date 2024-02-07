namespace TheElectrician.Models.Settings;

public class WireSettings : WireConnectableSettings
{
    public WireSettings(Type type, int startLevel, int maxLevel, float conductivity, float powerLoss, int maxConnections) :
        base(type, startLevel, maxLevel, conductivity, powerLoss, maxConnections)
    {
    }
}