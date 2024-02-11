using TheElectrician.Models.Settings;
using TheElectrician.Objects.Mono.Wire;

namespace TheElectrician.Objects;

public abstract class WireConnectable : Levelable, IWireConnectable
{
    public UnityEvent onConnectionsChanged { get; private set; }

    public float GetConductivity() => wireConnectableSettings.conductivity;

    private WireConnectableSettings wireConnectableSettings;
    protected HashSet<IPipeableConnectable> cashedConnections { get; private set; }

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

    public float GetPowerLoss() => wireConnectableSettings.powerLoss;

    public PipeTransferMode GetTransferMode() => PipeTransferMode.Power;

    public virtual HashSet<IPipeableConnectable> GetConnections()
    {
        if (IsValid() == false) return cashedConnections;
        var savedString = GetZDO().GetString(Consts.connectionsKey, "-1");
        if (savedString == "-1")
        {
            cashedConnections = [];
            return cashedConnections;
        }

        cashedConnections = savedString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x =>
        {
            if (Guid.TryParse(x, out var guid))
                return Library.GetObject(guid) as IPipeableConnectable;

            DebugError($"Failed to parse guid: '{x}'");
            return null;
        }).ToHashSet();
        return cashedConnections;
    }

    public virtual void AddConnection(IPipeableConnectable connectable)
    {
        if (connectable is null) return;
        if (!CanConnect(connectable)) return;
        cashedConnections.Add(connectable);
        UpdateConnections();
        connectable.AddConnection(this);
    }

    public virtual void RemoveConnection(IPipeableConnectable connectable)
    {
        if (connectable is null) return;
        if (!cashedConnections.Contains(connectable)) return;
        cashedConnections.Remove(connectable);
        UpdateConnections();
        connectable.RemoveConnection(this);
    }

    public virtual void SetConnections(HashSet<IPipeableConnectable> connections) => cashedConnections = connections;

    public virtual bool CanConnectOnlyToWires() => false;

    public virtual int GetMaxConnections() => wireConnectableSettings.maxConnections;

    public bool CanConnect(IPipeableConnectable connectable)
    {
        if (connectable is null) return false;
        if (connectable.GetTransferMode() != GetTransferMode()) return false;
        if (cashedConnections.Contains(connectable)) return false;

        if (CanConnectOnlyToWires() && connectable is not Wire) return false;
        if (cashedConnections.Count >= GetMaxConnections()) return false;

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