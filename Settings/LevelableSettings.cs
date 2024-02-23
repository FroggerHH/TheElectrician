namespace TheElectrician.Settings;

public class LevelableSettings(Type type, int startLevel, int maxLevel) : ElectricObjectSettings(type)
{
    public readonly int startLevel = startLevel;
    public readonly int maxLevel = maxLevel;

    public override string ToString() =>
        $"{base.ToString()} "
        + $"maxLevel={maxLevel} "
        + $"startLevel={startLevel}";
}