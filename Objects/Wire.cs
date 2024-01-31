using TheElectrician.Models;
using TheElectrician.Models.Settings;
using TheElectrician.Objects.Mono.Wire;
using UnityEngine.Events;

namespace TheElectrician.Objects;

public class Wire : ElectricObject, IWire
{
    private HashSet<IWireConnectable> cashedConnections;
    private WireState state;
    private WireSettings wireSettings;
    public UnityEvent onConnectionsChanged { get; private set; }

    public override void InitData()
    {
        base.InitData();
        GetConnections();
        GetConductivity();
        onConnectionsChanged = new UnityEvent();
    }

    public override void InitSettings(ElectricObjectSettings settings)
    {
        base.InitSettings(settings);
        wireSettings = GetSettings<WireSettings>();
        if (wireSettings is null)
            DebugError($"Wire.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not WireSettings");
    }

    public float GetConductivity()
    {
        var conductivity = GetZDO().GetFloat(Consts.wireConductivityKey, -1);
        if (conductivity == -1)
        {
            conductivity = wireSettings.conductivity;
            SetConductivity(conductivity);
        }

        return conductivity;
    }

    public void SetConductivity(float conductivity) { GetZDO().Set(Consts.wireConductivityKey, conductivity); }

    public WireState GetState() { return state; }

    public void SetState(WireState newState) { state = newState; }

    public HashSet<IWireConnectable> GetConnections()
    {
        if (IsValid() == false) return cashedConnections;
        var savedString = GetZDO().GetString(Consts.connectionsKey, "-1");
        if (savedString == "-1")
        {
            cashedConnections = new HashSet<IWireConnectable>();
            return cashedConnections;
        }

        cashedConnections = savedString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x =>
        {
            if (Guid.TryParse(x, out var guid))
                return Library.GetObject(guid) as IWireConnectable;

            DebugError($"Failed to parse guid: '{x}'");
            return null;
        }).ToHashSet();
        return cashedConnections;
    }

    public void AddConnection(IWireConnectable connectable)
    {
        if (connectable is null) return;
        if (cashedConnections.Contains(connectable)) return;
        cashedConnections.Add(connectable);
        UpdateConnections();
        connectable.AddConnection(this);
        SetState(WireState.Idle);
    }

    public void RemoveConnection(IWireConnectable connectable)
    {
        if (connectable is null) return;
        if (!cashedConnections.Contains(connectable)) return;
        cashedConnections.Remove(connectable);
        UpdateConnections();
        SetState(WireState.Idle);
        connectable.RemoveConnection(this);
    }

    public void SetConnections(HashSet<IWireConnectable> connections)
    {
        cashedConnections = connections;
        UpdateConnections();
    }

    public bool CanConnectOnlyToWires() { return false; }

    public virtual int MaxConnections() { return Consts.defaultWireMaxConnections; }

    public override string ToString()
    {
        if (!IsValid()) return "Uninitialized Wire";
        string connectedListStr;
        if (cashedConnections is null) connectedListStr = "none";
        else
            connectedListStr = cashedConnections.Count > 0
                ? string.Join(", ", cashedConnections.Select(x => x?.GetId().ToString() ?? "none"))
                : "no one";

        return $"Wire {GetId()} connected to: '{connectedListStr}'";
    }


    private void UpdateConnections()
    {
        cashedConnections = cashedConnections.Where(x => x is not null).ToHashSet();
        if (!IsValid()) return;
        var joinedStr = string.Join(";", cashedConnections.Select(x => x.GetId()));
        GetZDO().Set(Consts.connectionsKey, joinedStr);
        onConnectionsChanged?.Invoke();
    }
}