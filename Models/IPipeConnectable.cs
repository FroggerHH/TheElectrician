namespace TheElectrician.Models;

public interface IPipeConnectable : ILevelable
{
    UnityEvent onConnectionsChanged { get; }
    int GetMaxConnections();
    PipeTransferMode GetTransferMode();

    HashSet<IPipeConnectable> GetConnections();
    void AddConnection(IPipeConnectable connectable);
    void RemoveConnection(IPipeConnectable connectable);
    void SetConnections(HashSet<IPipeConnectable> connections);

    bool CanConnect(IPipeConnectable connectable);
}