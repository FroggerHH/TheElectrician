namespace TheElectrician.Settings;

public class WireSettings : WireConnectableSettings
{
    public WireSettings(Type type, int startLevel, int maxLevel, float conductivity, float powerLoss, int maxConnections, float maxDistance) :
        base(type, startLevel, maxLevel, conductivity, powerLoss, maxConnections, maxDistance)
    {
    }
}