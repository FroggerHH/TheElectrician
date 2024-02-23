using TheElectrician.Settings;

namespace TheElectrician.Objects;

public abstract class WireConnectable : PipeConnectable, IWireConnectable
{
    private WireConnectableSettings wireConnectableSettings;

    public override void InitSettings(ElectricObjectSettings sett)
    {
        base.InitSettings(sett);
        wireConnectableSettings = GetSettings<WireConnectableSettings>();
        if (wireConnectableSettings is null)
            DebugError(
                $"WireConnectable.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not WireConnectableSettings");
    }

    public float GetPowerLoss() => wireConnectableSettings.powerLoss;

    public override PipeTransferMode GetTransferMode() => PipeTransferMode.Power;

    public virtual bool CanConnectOnlyToWires() => false;

    public override bool CanConnect(IPipeConnectable connectable)
    {
        if (connectable is not null && CanConnectOnlyToWires() && connectable is not Wire) return false;
        return base.CanConnect(connectable);
    }
}