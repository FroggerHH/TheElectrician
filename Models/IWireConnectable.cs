using UnityEngine.Events;

namespace TheElectrician.Models;

public interface IWireConnectable : IElectricObject
{
    UnityEvent onConnectionsChanged { get; }
    HashSet<IWireConnectable> GetConnections();
    void AddConnection(IWireConnectable connectable);
    void RemoveConnection(IWireConnectable connectable);
    void SetConnections(HashSet<IWireConnectable> connections);
    bool CanConnectOnlyToWires();
    int MaxConnections();
}