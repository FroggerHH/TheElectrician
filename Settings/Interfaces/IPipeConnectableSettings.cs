namespace TheElectrician.Settings.Interfaces;

public interface IPipeConnectableSettings : IElectricObjectSettings
{
    int maxConnections { get; }
}