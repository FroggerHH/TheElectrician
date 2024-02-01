using TheElectrician.Models;
using TheElectrician.Objects;

namespace TheElectrician.Systems;

internal static class EOLifeHandler
{
    private static readonly Dictionary<Guid, ElectricObject> m_eoByID = new();
    private static readonly Dictionary<ZDO, ElectricObject> m_eoByZdo = new();
    public static bool worldEOsLoaded { get; private set; }

    public static async void Load()
    {
        worldEOsLoaded = true;
        var worldObjects = await ZoneSystem.instance.GetWorldObjectsAsync(x => Library.IsEO(x.GetPrefab()));
        foreach (var zdo in worldObjects) CreateNewEO(zdo, out _);
    }

    internal static bool CreateNewEO(ZDO zdo, out ElectricObject newEO)
    {
        newEO = null;
        if (GetObject(zdo) != null) return false;
        newEO = EOPool.Create(zdo);
        if (newEO is null) return false;
        m_eoByID.Add(newEO.GetId(), newEO);
        m_eoByZdo.Add(zdo, newEO);
        return true;
    }

    internal static void DestroyEO(ElectricObject eo)
    {
        m_eoByID.Remove(eo.GetId());
        m_eoByZdo.Remove(eo.GetZDO());
        EOPool.Release(eo);
    }

    public static void DestroyEO(ZDO zdo)
    {
        var eo = GetObject(zdo);
        if (eo != null)
            DestroyEO(eo);
    }

    internal static List<IElectricObject> GetAllObjects() { return new List<IElectricObject>(m_eoByID.Values); }

    internal static List<T> GetAllObjects<T>() where T : IElectricObject
    {
        return m_eoByID.Values.OfType<T>().ToList();
    }

    internal static ElectricObject GetObject(Guid id) { return m_eoByID.TryGetValue(id, out var eo) ? eo : null; }

    internal static ElectricObject GetObject(ZDO zdo) { return m_eoByZdo.TryGetValue(zdo, out var eo) ? eo : null; }

    internal static void Clear()
    {
        m_eoByID.Clear();
        m_eoByZdo.Clear();
        EOPool.ReleaseAll();
    }
}