namespace TheElectrician.Models.Settings;

[Serializable]
public class ElectricObjectSettings(Type type)
{
    public readonly Type type = type;

    public override string ToString() => $"Settings: type={type.Name}";
}