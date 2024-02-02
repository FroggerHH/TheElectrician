using TheElectrician.Models;
using TheElectrician.Models.Settings;
using TheElectrician.Objects.Mono.Wire;
using UnityEngine.Events;

namespace TheElectrician.Objects;

public abstract class WireConnectable : Levelable, IWireConnectable
{
    public UnityEvent onConnectionsChanged { get; private set; }

    public float GetConductivity() => wireConnectableSettings.conductivity;

    private WireConnectableSettings wireConnectableSettings;
    protected HashSet<IWireConnectable> cashedConnections { get; private set; }

    public override void InitSettings(ElectricObjectSettings sett)
    {
        base.InitSettings(sett);
        wireConnectableSettings = GetSettings<WireConnectableSettings>();
        if (wireConnectableSettings is null)
            DebugError(
                $"WireConnectable.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not WireConnectableSettings");
    }

    public override void InitData()
    {
        base.InitData();
        GetConnections();
        onConnectionsChanged = new UnityEvent();
    }

    public virtual HashSet<IWireConnectable> GetConnections()
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

    public virtual void AddConnection(IWireConnectable connectable)
    {
        if (connectable is null) return;
        if (!CanConnect(connectable)) return;
        cashedConnections.Add(connectable);
        UpdateConnections();
        connectable.AddConnection(this);
    }

    public virtual void RemoveConnection(IWireConnectable connectable)
    {
        if (connectable is null) return;
        if (!cashedConnections.Contains(connectable)) return;
        cashedConnections.Remove(connectable);
        UpdateConnections();
        connectable.RemoveConnection(this);
    }

    public virtual void SetConnections(HashSet<IWireConnectable> connections) => cashedConnections = connections;

    public virtual bool CanConnectOnlyToWires() => false;

    public virtual int MaxConnections() => wireConnectableSettings.maxConnections;

    public bool CanConnect(IWireConnectable connectable)
    {
        if (connectable is null) return false;
        if (cashedConnections.Contains(connectable)) return false;

        if (CanConnectOnlyToWires() && connectable is not Wire) return false;
        if (cashedConnections.Count >= MaxConnections()) return false;

        return true;
    }

    protected virtual void UpdateConnections()
    {
        cashedConnections = cashedConnections.Where(x => x is not null).ToHashSet();
        if (!IsValid()) return;
        var joinedStr = string.Join(";", cashedConnections.Select(x => x.GetId()));
        GetZDO().Set(Consts.connectionsKey, joinedStr);
        onConnectionsChanged?.Invoke();
    }
}