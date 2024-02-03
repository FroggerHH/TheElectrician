using TheElectrician.Models;
using TheElectrician.Models.Settings;
using TheElectrician.Objects.Mono.Wire;
using UnityEngine.Events;

namespace TheElectrician.Objects;

public class Wire : WireConnectable, IWire
{
    private WireState state;
    private WireSettings wireSettings;

    public override void InitSettings(ElectricObjectSettings settings)
    {
        base.InitSettings(settings);
        wireSettings = GetSettings<WireSettings>();
        if (wireSettings is null)
            DebugError($"Wire.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not WireSettings");
    }

    public WireState GetState() => state;

    public void SetState(WireState newState) => state = newState;

    public override void AddConnection(IWireConnectable connectable)
    {
        base.AddConnection(connectable);
        SetState(WireState.Idle);
    }

    public override void RemoveConnection(IWireConnectable connectable)
    {
        base.RemoveConnection(connectable);
        SetState(WireState.Idle);
    }

    public override bool CanConnectOnlyToWires() { return false; }

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
}