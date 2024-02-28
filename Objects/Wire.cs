using TheElectrician.Objects.Mono.Wire;
using TheElectrician.Settings;
using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Objects;

public class Wire : WireConnectable, IWire
{
    private WireState state;
    private IWireSettings wireSettings;

    public override void InitSettings(IElectricObjectSettings settings)
    {
        base.InitSettings(settings);
        wireSettings = GetSettings<IWireSettings>();
        if (wireSettings is null)
            DebugError($"{GetType().Name}.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not IWireSettings");
    }

    public WireState GetState() => state;

    public void SetState(WireState newState) => state = newState;

    public override void AddConnection(IPipeConnectable connectable)
    {
        base.AddConnection(connectable);
        SetState(WireState.Idle);
    }

    public override void RemoveConnection(IPipeConnectable connectable)
    {
        base.RemoveConnection(connectable);
        SetState(WireState.Idle);
    }

    public override bool CanConnectOnlyToWires() { return false; }
}