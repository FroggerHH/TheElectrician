using TheElectrician.Models;
using TheElectrician.Models.Settings;
using TheElectrician.Objects.Mono.Wire;
using UnityEngine.Events;

namespace TheElectrician.Objects;

public class Wire : IWire
{
    private readonly ZDO zdo;
    private HashSet<IElectricObject> cashedConnections;
    private WireState state;
    public UnityEvent onConnectionsChanged { get; }

    public void UpdateConnectionsList()
    {
        GetConnections();
        onConnectionsChanged?.Invoke();
    }

    public Wire(ZDO zdo)
    {
        this.zdo = zdo;
        Library.TryGiveId(this);
        GetConnections();
        onConnectionsChanged = new();
        onConnectionsChanged.AddListener(() => Debug($"Wire {GetId()} connections changed"));
    }


    public ZDO GetZDO() => zdo;

    public void Update() { }

    public void InitSettings(ElectricObjectSettings settings)
    {
        if (settings is not WireSettings wireSettings) return;
        SetConductivity(wireSettings.conductivity);
    }

    public Guid GetId() => Guid.Parse(zdo.GetString(Consts.electricObjectIdKey, Guid.Empty.ToString()));

    public HashSet<IElectricObject> GetConnections()
    {
        var savedString = GetZDO().GetString(Consts.storageKey, "-1");
        if (savedString == "-1")
        {
            cashedConnections = new();
            return cashedConnections;
        }

        cashedConnections = savedString.Split(';').Select(x =>
        {
            if (Guid.TryParse(x, out var guid))
                return Library.GetObject(guid) as IElectricObject;
            else
            {
                DebugError($"Failed to parse guid: '{x}'");
                return null;
            }
        }).ToHashSet();
        return cashedConnections;
    }


    public void AddConnection(IElectricObject electricObject)
    {
        Debug($"Adding connection: {electricObject?.GetId().ToString() ?? "null"} to {GetId()}");
        if (electricObject is null) return;
        if (cashedConnections.Contains(electricObject)) return;
        cashedConnections.Add(electricObject);
        if (electricObject is IWire)
            ((IWire)electricObject).AddConnection(this);
        else if (electricObject is Storage)
            ((IStorage)electricObject).AddConnectionWire(this);

        UpdateConnections();
    }

    public void RemoveConnection(IElectricObject electricObject)
    {
        if (electricObject is null) return;
        if (!cashedConnections.Contains(electricObject)) return;
        cashedConnections.Remove(electricObject);
        if (electricObject is IWire wire) wire.RemoveConnection(this);
        else if (electricObject is IStorage storage) storage.RemoveConnectionWire(this);
        UpdateConnections();
    }

    public float GetConductivity() => zdo.GetFloat(Consts.wireConductivityKey);

    public void SetConductivity(float conductivity) => zdo.Set(Consts.wireConductivityKey, conductivity);

    public WireState GetState() { return state; }

    public void SetState(WireState newState) => this.state = newState;

    private void UpdateConnections()
    {
        cashedConnections = cashedConnections.Where(x => x is not null).ToHashSet();
        GetZDO().Set(Consts.storageKey, string.Join(";", cashedConnections.Select(x => x.GetId())));
        onConnectionsChanged?.Invoke();
    }

    public override string ToString()
    {
        string connectedListStr;
        if (cashedConnections is null) connectedListStr = "none";
        else
            connectedListStr = cashedConnections.Count > 0
                ? string.Join(", ", cashedConnections.Select(x => x?.GetId().ToString() ?? "none"))
                : "no one";

        return $"Wire {GetId()} connected to: '{connectedListStr}'";
    }
}