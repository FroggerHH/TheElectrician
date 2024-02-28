namespace TheElectrician.Settings.Interfaces;

public interface IItemPipeSettings : IItemPipeConnectableSettings
{
    int maxWeight { get; }
    int maxDifferentItemsCount { get; }
}