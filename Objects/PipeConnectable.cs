using TheElectrician.Settings;
using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Objects;

public abstract class PipeConnectable : Levelable, IPipeConnectable
{
    public UnityEvent onConnectionsChanged { get; private set; }

    private IPipeConnectableSettings pipeConnectableSettings;
    private HashSet<IPipeConnectable> cashedConnections;

    public override void InitSettings(IElectricObjectSettings sett)
    {
        base.InitSettings(sett);
        pipeConnectableSettings = GetSettings<IPipeConnectableSettings>();
        if (pipeConnectableSettings is null)
            DebugError(
                $"{GetType().Name}.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not IPipeConnectableSettings");
    }

    public override void InitData()
    {
        base.InitData();
        GetConnections();
        onConnectionsChanged = new UnityEvent();
    }

    public int GetMaxConnections() => pipeConnectableSettings.maxConnections;

    public abstract PipeTransferMode GetTransferMode();

    public HashSet<IPipeConnectable> GetConnections()
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
                return Library.GetObject(guid) as IPipeConnectable;

            DebugError($"Failed to parse guid: '{x}'");
            return null;
        }).ToHashSet();
        return cashedConnections;
    }

    public virtual void AddConnection(IPipeConnectable connectable)
    {
        if (connectable is null) return;
        if (!CanConnect(connectable)) return;
        cashedConnections.Add(connectable);
        UpdateConnections();
        connectable.AddConnection(this);
    }

    public virtual void RemoveConnection(IPipeConnectable connectable)
    {
        if (connectable is null) return;
        if (!cashedConnections.Contains(connectable)) return;
        cashedConnections.Remove(connectable);
        UpdateConnections();
        connectable.RemoveConnection(this);
    }

    public virtual void SetConnections(HashSet<IPipeConnectable> connections) => cashedConnections = connections;

    public virtual bool CanConnect(IPipeConnectable connectable)
    {
        if (connectable is null) return false;
        if (connectable.GetTransferMode() != GetTransferMode()) return false;
        if (cashedConnections.Contains(connectable)) return false;

        if (Max(cashedConnections.Count, connectable.GetConnections().Count)
            >= Max(GetMaxConnections(), connectable.GetMaxConnections())) return false;

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