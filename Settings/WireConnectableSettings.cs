namespace TheElectrician.Settings;

public class WireConnectableSettings(
    Type type,
    int startLevel,
    int maxLevel,
    float conductivity,
    float powerLoss,
    int maxConnections,
    float maxDistance)
    : PipeConnectableSettings(type, startLevel, maxLevel, conductivity, maxConnections, maxDistance)
{
    public readonly float powerLoss = powerLoss;

    public override string ToString() => $"{base.ToString()} powerLoss={powerLoss}";
}