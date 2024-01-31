using TheElectrician.Models;

namespace TheElectrician.Objects;

public class PowerSystem
{
    private HashSet<IWireConnectable> connections = new();

    public float GetPowerStored() { return connections.OfType<IStorage>().Sum(x => x.Count(Consts.storagePowerKey)); }

    public override string ToString() { return $"Connections: {connections.GetString()}, Power: {GetPowerStored()}"; }

    public bool ContainsConnection(IWireConnectable element) { return connections.Contains(element); }

    public HashSet<IWireConnectable> GetConnections() { return connections; }

    public void SetConnections(HashSet<IWireConnectable> connections) { this.connections = connections; }
}