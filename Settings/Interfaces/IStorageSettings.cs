namespace TheElectrician.Settings.Interfaces;

public interface IStorageSettings : IElectricObjectSettings
{
    int powerCapacity { get; }
    int otherCapacity { get; }
    string[] allowedKeys { get; }
}