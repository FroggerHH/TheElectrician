namespace TheElectrician.Models;

public interface IWireConnectable : IPipeableConnectable
{
    float GetConductivity();
    float GetPowerLoss();
    bool CanConnectOnlyToWires();
}