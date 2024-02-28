using TheElectrician.Settings;
using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Objects;

public abstract class ItemPipeConnectable : PipeConnectable, IItemPipeConnectable
{
    private IItemPipeConnectableSettings itemPipeConnectableSettings;

    public override void InitSettings(IElectricObjectSettings sett)
    {
        base.InitSettings(sett);
        itemPipeConnectableSettings = GetSettings<IItemPipeConnectableSettings>();
        if (itemPipeConnectableSettings is null)
            DebugError(
                $"{GetType().Name}.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not IItemPipeConnectableSettings");
    }

    public override PipeTransferMode GetTransferMode() => PipeTransferMode.Items;
}