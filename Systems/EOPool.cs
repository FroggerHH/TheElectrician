using TheElectrician.Objects;

namespace TheElectrician.Systems;

public static class EOPool
{
    private const int c_BatchSize = 3;
    private static readonly Dictionary<Type, Stack<ElectricObject>> free = new();
    private static int s_active;

    public static void Init()
    {
        free.Clear();
        var types = Library.GetAllSettings().Values.Select(x => x.type).ToList();
        foreach (var t in types) free.Add(t, new Stack<ElectricObject>());
    }

    public static ElectricObject Create(ZDO zdo)
    {
        var settings = Library.GetSettings(zdo.GetPrefab());
        if (settings is null)
        {
            DebugError($"Settings for {zdo.GetPrefab()} not found!");
            return null;
        }

        var eo = Get(settings.type);
        eo.Init(zdo);
        eo.InitSettings(settings);
        eo.InitData();
        return eo;
    }

    public static void Release(Dictionary<Guid, ElectricObject> objects)
    {
        foreach (var eo in objects.Values) Release(eo);
    }

    public static void Release(ElectricObject eo)
    {
        eo.Reset();
        free[eo.GetType()].Push(eo);
        --s_active;
    }

    public static ElectricObject Get(Type type)
    {
        if (free[type].Count <= 0)
            for (var index = 0; index < c_BatchSize; ++index)
            {
                var eo = Activator.CreateInstance(type) as ElectricObject;
                free[type].Push(eo);
            }

        ++s_active;
        var eo1 = free[type].Pop();
        return eo1;
    }

    public static int GetPoolSize() { return free.Count; }

    public static int GetPoolActive() { return s_active; }

    public static int GetPoolTotal() { return s_active + free.Count; }

    public static void ReleaseAll()
    {
        foreach (var f in free)
        foreach (var o in f.Value)
            Release(o);
    }
}