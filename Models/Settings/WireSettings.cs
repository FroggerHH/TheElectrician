namespace TheElectrician.Models.Settings;

public class WireSettings : ElectricObjectSettings
{
    public readonly float conductivity;

    public WireSettings(Type type, float conductivity) : base(type) { this.conductivity = conductivity; }
}