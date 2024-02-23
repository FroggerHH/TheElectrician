namespace TheElectrician.Models;

public interface IWireConnectable : IPipeConnectable
{
    float GetPowerLoss();
    bool CanConnectOnlyToWires();
}