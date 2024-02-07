namespace TheElectrician.Models.Settings;

public class ConsumerSettings : WireConnectableSettings
{
    public ConsumerSettings(Type type, int startLevel, int maxLevel, float conductivity, float powerLoss, int maxConnections) : base(type, startLevel, maxLevel, conductivity, powerLoss, maxConnections)
    {
    }
}