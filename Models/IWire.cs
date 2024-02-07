using TheElectrician.Objects.Mono.Wire;

namespace TheElectrician.Models;

public interface IWire : IWireConnectable
{
    WireState GetState();
    void SetState(WireState newState);
}