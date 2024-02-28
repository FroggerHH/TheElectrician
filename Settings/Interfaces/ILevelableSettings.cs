namespace TheElectrician.Settings.Interfaces;

public interface ILevelableSettings : IElectricObjectSettings
{
    int startLevel { get; }
    int maxLevel { get; }
}