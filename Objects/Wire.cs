using TheElectrician.Models.Settings;
using TheElectrician.Objects.Mono.Wire;

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

    public override void AddConnection(IPipeableConnectable connectable)
    {
        base.AddConnection(connectable);
        SetState(WireState.Idle);
    }

    public override void RemoveConnection(IPipeableConnectable connectable)
    {
        base.RemoveConnection(connectable);
        SetState(WireState.Idle);
    }

    public override bool CanConnectOnlyToWires() { return false; }
}