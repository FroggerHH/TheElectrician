namespace TheElectrician.Models;

public interface IWireConnectable : IPipeConnectable
{
    float GetConductivity();
    float GetPowerLoss();
    bool CanConnectOnlyToWires();
}