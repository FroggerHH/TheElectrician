using TheElectrician.Settings;

namespace TheElectrician.Objects;

public abstract class ItemPipeConnectable : PipeConnectable, IItemPipeConnectable
{
    private ItemPipeConnectableSettings itemPipeConnectableSettings;

    public override void InitSettings(ElectricObjectSettings sett)
    {
        base.InitSettings(sett);
        itemPipeConnectableSettings = GetSettings<ItemPipeConnectableSettings>();
        if (itemPipeConnectableSettings is null)
            DebugError(
                $"ItemPipeConnectable.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not ItemPipeConnectableSettings");
    }

    public override PipeTransferMode GetTransferMode() => PipeTransferMode.Items;

    public int GetMaxWeight() => itemPipeConnectableSettings.maxWeight;

    public int GetMaxItemsCount() => itemPipeConnectableSettings.maxDifferentItemsCount;
}