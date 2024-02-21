namespace TheElectrician.Models;

public interface IItemsPipe : IPipeableConnectable
{
}

public interface IItemsPipeConnectable : IPipeableConnectable
{
    int GetMaxWeight();
    int GetMaxItemsCount();
}