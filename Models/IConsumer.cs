namespace TheElectrician.Models;

public interface IConsumer : IElectricObject
{
    float GetPossiblePower();
}