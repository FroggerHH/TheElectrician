using TheElectrician.Models.Settings;

namespace TheElectrician.Objects;

public class ElectricObject : IElectricObject
{
    private bool isValid;
    private ZDO m_zdo;
    private ElectricObjectSettings settings;
    internal ElectricObject() { }
    public virtual ZDO GetZDO() { return m_zdo; }

    public virtual void Update() { }

    public virtual void InitSettings(ElectricObjectSettings settings) { this.settings = settings; }

    public Guid GetId()
    {
        if (GetZDO() is null) return Guid.Empty;
        return Guid.Parse(GetZDO().GetString(Consts.electricObjectIdKey, Guid.Empty.ToString()));
    }

    public virtual void InitData() { }

    public bool IsValid() { return isValid && (GetZDO()?.IsValid() ?? false) && GetId() != Guid.Empty; }

    public override string ToString()
    {
        return $"EO, Type: {GetType()}, IsValid: {isValid}, zdo: {m_zdo}, "
               + $"settings: {settings?.ToString() ?? "null"}";
    }

    public T GetSettings<T>() where T : ElectricObjectSettings { return settings as T; }
    public ElectricObjectSettings GetSettings() { return settings; }

    internal void Reset()
    {
        isValid = false;
        m_zdo = null;

        if (this is not IWireConnectable wireConnectable) return;
        List<IPipeableConnectable> connections = [];
        foreach (var connection in wireConnectable.GetConnections()) connections.Add(connection);
        Debug($"Reset {wireConnectable.ToString() ?? "null"}, "
              + $"connections: {wireConnectable.GetConnections().GetString()}");
        foreach (var con in connections) con.RemoveConnection(wireConnectable);
    }

    internal void Init(ZDO zdo)
    {
        m_zdo = zdo;
        if (GetId() == Guid.Empty) SetId(Guid.NewGuid());
        isValid = true;
    }

    private void SetId(Guid id) { GetZDO().Set(Consts.electricObjectIdKey, id.ToString()); }
}