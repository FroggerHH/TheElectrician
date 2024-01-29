using TheElectrician.Objects.Mono.Wire;
using UnityEngine.Events;

namespace TheElectrician.Models;

public interface IWire : IElectricObject
{
    HashSet<IElectricObject> GetConnections();
    void AddConnection(IElectricObject wire);
    void RemoveConnection(IElectricObject electricObject);

    float GetConductivity();
    void SetConductivity(float conductivity);
    WireState GetState();
    void SetState(WireState newState);

    UnityEvent onConnectionsChanged { get; }
    void UpdateConnectionsList();
}