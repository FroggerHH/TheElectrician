namespace TheElectrician.Settings.Interfaces;

public interface IGeneratorSettings : IStorageSettings
{
    string fuelItem { get; }
    float fuelPerTick { get; }
    int maxFuel { get; }
    float powerPerTick { get; }
}