using TheElectrician.Objects.Mono.Wire;

namespace TheElectrician.Models;

public interface IWire : IWireConnectable
{
    float GetConductivity();
    void SetConductivity(float conductivity);
    WireState GetState();
    void SetState(WireState newState);
}