using BepInEx.Configuration;

namespace TheElectrician.Systems.Config;

[PublicAPI]
public static class TheConfig
{
    private static bool debugConfig;
    private static ConfigEntry<bool> _debugConfigConfig;

    private static bool _isInitialized;
    private static ConfigEntry<float> _wireUpdateCableIntervalConfig;
    private static ConfigEntry<float> _wireMaxFalloffByDistanceConfig;
    private static ConfigEntry<float> _objectTickTimeConfig;
    private static ConfigEntry<float> _powerTickTimeConfig;

    public static float WireUpdateCableInterval { get; private set; }

    public static float WireMaxFalloffByDistance { get; private set; }

    public static float ObjectTickTime { get; private set; } = 999;

    public static float PowerTickTime { get; private set; } = 999;

    internal static void Init()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        _debugConfigConfig = config("Debug", "Debug Config", false, "Debug config");

        _wireUpdateCableIntervalConfig = config("Wires", "Update Cables Interval", 2f,
            new ConfigDescription(
                "How often to update cable power. Cables are just the visual. It does not affect power generation. "
                + "This setting was added for people who have performance issues with too many cables.",
                new AcceptableValueRange<float>(1f, 5f)));

        _wireMaxFalloffByDistanceConfig = config("Wires", "Max Falloff By Distance", 0.4f,
            new ConfigDescription("", new AcceptableValueRange<float>(0f, 1f)));

        _objectTickTimeConfig = config("General", "Objects Tick Time", 0.5f,
            new ConfigDescription("The time between updating objects. In seconds",
                new AcceptableValueRange<float>(0.01f, 2f)));

        _powerTickTimeConfig = config("General", "Power Tick Time", 0.5f,
            new ConfigDescription("The time between updating power flow. In seconds",
                new AcceptableValueRange<float>(0.01f, 2f)));
    }


    internal static void UpdateConfiguration()
    {
        debugConfig = _debugConfigConfig.Value;
        WireUpdateCableInterval = _wireUpdateCableIntervalConfig.Value;
        WireMaxFalloffByDistance = _wireMaxFalloffByDistanceConfig.Value;
        ObjectTickTime = _objectTickTimeConfig.Value;
        PowerTickTime = _powerTickTimeConfig.Value;

        if (debugConfig) Debug("Configuration updated");
    }
}