using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Settings;

[Serializable]
public abstract record ElectricObjectSettings(Type type) : IElectricObjectSettings
{
    public override string ToString() => $"Settings: type={type.Name}";
    public Type type { get; } = type;
}