namespace TheElectrician.Models;

public interface IPipeableConnectable : ILevelable
{
    UnityEvent onConnectionsChanged { get; }
    int GetMaxConnections();
    PipeTransferMode GetTransferMode();

    HashSet<IPipeableConnectable> GetConnections();
    void AddConnection(IPipeableConnectable connectable);
    void RemoveConnection(IPipeableConnectable connectable);
    void SetConnections(HashSet<IPipeableConnectable> connections);

    bool CanConnect(IPipeableConnectable connectable);
}