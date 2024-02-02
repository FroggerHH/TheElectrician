namespace TheElectrician.Models.Settings;

public class LevelableSettings : ElectricObjectSettings
{
    public readonly int startLevel;
    public readonly int maxLevel;

    public LevelableSettings(Type type, int startLevel, int maxLevel) : base(type)
    {
        this.startLevel = startLevel;
        this.maxLevel = maxLevel;
    }

    public override string ToString()
    {
        return $"{base.ToString()} "
               + $"maxLevel={maxLevel} "
               + $"startLevel={startLevel}";
    }
}