namespace TheElectrician.Models;

public interface IItemPipe : IItemPipeConnectable
{
    int GetMaxWeight();
    int GetMaxItemsCount();
}