namespace TheElectrician.Settings.Interfaces;

public interface IWireConnectableSettings : IPipeConnectableSettings
{
    float powerLoss { get; }
    int conductivity { get; }
    float maxDistance { get; }
}