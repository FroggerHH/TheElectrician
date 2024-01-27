using TheElectrician.Models;
using TheElectrician.Models.Settings;

namespace TheElectrician.Systems;

public static class Library
{
    private static readonly List<IElectricObject> AllObjects = new();
    private static readonly Dictionary<int, ElectricObjectSettings> AllTypes = new();

    public static void Register(string name, ElectricObjectSettings settings)
    {
        Register(name.GetStableHashCode(), settings);
    }

    public static void Register(int name, ElectricObjectSettings settings)
    {
        if (AllTypes.ContainsKey(name))
        {
            DebugError($"Library already contains key: {name}");
            return;
        }

        AllTypes[name] = settings;
    }

    public static ElectricObjectSettings GetSettings(int name) { return AllTypes[name]; }

    public static ElectricObjectSettings GetSettings(string name) { return GetSettings(name.GetStableHashCode()); }

    public static bool TryGetSettings(string name, out ElectricObjectSettings type)
    {
        return TryGetSettings(name.GetStableHashCode(), out type);
    }

    public static bool TryGetSettings(int name, out ElectricObjectSettings type)
    {
        return AllTypes.TryGetValue(name, out type);
    }

    public static List<IElectricObject> GetAllObjects() { return AllObjects; }


    public static void AddObject(IElectricObject obj)
    {
        if (!AllObjects.Contains(obj)) AllObjects.Add(obj);
    }

    public static void RemoveObject(IElectricObject obj)
    {
        if (obj is null) return;
        AllObjects.Remove(obj);
    }

    public static void SpawnObject(ZDO zdo)
    {
        if (!CreateObject(zdo, out var obj)) return;
        AddObject(obj);
    }

    private static bool CreateObject(ZDO zdo, out IElectricObject obj)
    {
        obj = null;
        if (!TryGetSettings(zdo.GetPrefab(), out var settings)) return false;
        obj = Activator.CreateInstance(settings.type, zdo) as IElectricObject;
        if (obj is null)
        {
            DebugError($"Failed to create object: {zdo.GetPrefab()}, type: {settings?.type?.ToString() ?? "null"}");
            return false;
        }

        obj.InitSettings(settings);

        Debug($"Object created: {obj}");
        return true;
    }

    public static async void AddObjectsFromWorld()
    {
        var worldObjects = await ZoneSystem.instance.GetWorldObjectsAsync(x => AllTypes.ContainsKey(x.GetPrefab()));
        foreach (var zdo in worldObjects)
        {
            if (AllObjects.Any(x => x.GetZDO() == zdo)) continue;

            var settings = GetSettings(zdo.GetPrefab());
            var obj = Activator.CreateInstance(settings.type, zdo) as IElectricObject;
            AllObjects.Add(obj);
        }
    }

    public static IElectricObject GetObject(ZDO zdo) { return AllObjects.FirstOrDefault(x => x.GetZDO() == zdo); }

    public static void Clear() { AllObjects.Clear(); }
}