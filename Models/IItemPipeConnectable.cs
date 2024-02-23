namespace TheElectrician.Models;

public interface IItemPipeConnectable : IPipeConnectable
{
    int GetMaxWeight();
    int GetMaxItemsCount();
}