namespace TheElectrician.Models.Settings;

[Serializable]
public class ElectricObjectSettings
{
    public readonly Type type;
    public ElectricObjectSettings(Type type) { this.type = type; }

    public override string ToString() { return $"Settings: type={type.Name}"; }
}