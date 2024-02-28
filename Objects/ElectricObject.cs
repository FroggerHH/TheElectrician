﻿using TheElectrician.Settings;
using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Objects;

public class ElectricObject : IElectricObject
{
    internal ElectricObject() { }

    private bool isValid;
    private ZDO m_zdo;
    private IElectricObjectSettings settings;
    public virtual ZDO GetZDO() { return m_zdo; }

    public virtual void Update() { }

    public virtual void InitSettings(IElectricObjectSettings settings) { this.settings = settings; }

    public Guid GetId()
    {
        if (GetZDO() is null) return Guid.Empty;
        return Guid.Parse(GetZDO().GetString(Consts.electricObjectIdKey, Guid.Empty.ToString()));
    }

    public virtual void InitData() { }

    public bool IsValid() { return isValid && (GetZDO()?.IsValid() ?? false) && GetId() != Guid.Empty; }

    public override string ToString()
    {
        if (!IsValid()) return $"Uninitialized {settings?.type.Name ?? $"{nameof(ElectricObject)}"}";
        return $"{settings?.type.Name} {GetId()}";
    }

    public T GetSettings<T>() where T : IElectricObjectSettings
    {
        if (settings is T result) return result;
        return default;
    }

    public IElectricObjectSettings GetSettings() { return settings; }

    internal void Reset()
    {
        isValid = false;
        m_zdo = null;

        if (this is not IWireConnectable wireConnectable) return;
        List<IPipeConnectable> connections = [];
        foreach (var connection in wireConnectable.GetConnections()) connections.Add(connection);
        //Debug($"Reset {wireConnectable.ToString() ?? "null"}, connections: {wireConnectable.GetConnections().GetString()}");
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