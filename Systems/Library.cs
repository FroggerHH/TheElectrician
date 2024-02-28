using TheElectrician.Settings;
using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Systems;

[PublicAPI]
public static class Library
{
    private static readonly Dictionary<int, IElectricObjectSettings> settingsMap = new();

    public static void Register(string name, IElectricObjectSettings settings)
    {
        Register(name.GetStableHashCode(), settings);
    }

    public static void Register(int name, IElectricObjectSettings settings)
    {
        if (settingsMap.ContainsKey(name))
        {
            DebugError($"Library already contains key: {name}");
            return;
        }

        settingsMap.Add(name, settings);
        EOPool.Init();
    }

    public static IElectricObjectSettings GetSettings(string name) { return GetSettings(name.GetStableHashCode()); }

    public static IElectricObjectSettings GetSettings(int name)
    {
        if (settingsMap.TryGetValue(name, out var settings))
            return settings;

        return null;
    }

    public static bool IsEO(string prefab) { return IsEO(prefab.GetStableHashCode()); }

    public static bool IsEO(int prefab) => settingsMap.ContainsKey(prefab);

    public static bool IsEO(GameObject go) { return IsEO(go.GetPrefabName()); }
    public static bool IsEO(Component go) { return IsEO(go.gameObject); }

    public static List<IElectricObject> GetAllObjects() { return EOLifeHandler.GetAllObjects(); }

    public static List<T> GetAllObjects<T>() where T : IElectricObject { return EOLifeHandler.GetAllObjects<T>(); }

    public static IElectricObject GetObject(Guid guid) { return EOLifeHandler.GetObject(guid); }
    public static IElectricObject GetObject(ZDO zdo) { return EOLifeHandler.GetObject(zdo); }

    public static Dictionary<int, IElectricObjectSettings> GetAllSettings() { return settingsMap; }
}