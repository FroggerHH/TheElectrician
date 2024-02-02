using UnityEngine.Events;

namespace TheElectrician.Models;

public interface IWireConnectable : ILevelable
{
    UnityEvent onConnectionsChanged { get; }

    float GetConductivity();
    HashSet<IWireConnectable> GetConnections();
    void AddConnection(IWireConnectable connectable);
    void RemoveConnection(IWireConnectable connectable);
    void SetConnections(HashSet<IWireConnectable> connections);
    bool CanConnectOnlyToWires();
    int MaxConnections();
    
    bool CanConnect(IWireConnectable connectable);
}