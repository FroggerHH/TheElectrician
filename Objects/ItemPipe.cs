using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Objects;

public class ItemPipe : ItemPipeConnectable, IItemPipe
{
    private IItemPipeSettings itemPipeSettings;

    public override void InitSettings(IElectricObjectSettings sett)
    {
        base.InitSettings(sett);
        itemPipeSettings = GetSettings<IItemPipeSettings>();
        if (itemPipeSettings is null)
            DebugError(
                $"{GetType().Name}.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not IItemPipeSettings");
    }

    public int GetMaxWeight() { throw new NotImplementedException(); }

    public int GetMaxItemsCount() { throw new NotImplementedException(); }
}