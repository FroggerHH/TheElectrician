using TheElectrician.Settings;
using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Objects;

public abstract class WireConnectable : PipeConnectable, IWireConnectable
{
    private IWireConnectableSettings wireConnectableSettings;

    public override void InitSettings(IElectricObjectSettings sett)
    {
        base.InitSettings(sett);
        wireConnectableSettings = GetSettings<IWireConnectableSettings>();
        if (wireConnectableSettings is null)
            DebugError(
                $"{GetType().Name}.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not IWireConnectableSettings");
    }

    public float GetConductivity() => wireConnectableSettings.conductivity;

    public float GetPowerLoss() => wireConnectableSettings.powerLoss;

    public override PipeTransferMode GetTransferMode() => PipeTransferMode.Power;

    public virtual bool CanConnectOnlyToWires() => false;

    public override bool CanConnect(IPipeConnectable connectable)
    {
        if (connectable is not null && CanConnectOnlyToWires() && connectable is not Wire) return false;
        return base.CanConnect(connectable);
    }
}