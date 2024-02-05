using TheElectrician.Models;

namespace TheElectrician.Systems.PowerFlow;

public class PowerSystem
{
    private HashSet<IWireConnectable> connections = new();

    public float GetPowerStored() { return connections.OfType<IStorage>().Sum(x => x.Count(Consts.storagePowerKey)); }

    public override string ToString() { return $"Connections: {connections.GetString()}, Power: {GetPowerStored()}"; }

    public bool ContainsConnection(IWireConnectable element) { return connections.Contains(element); }

    public HashSet<IWireConnectable> GetConnections() { return connections; }

    public void SetConnections(HashSet<IWireConnectable> connections) { this.connections = connections; }


    public float GetPossiblePowerInElement(IWireConnectable element)
    {
        if (element is null) return 0;
        if (!ContainsConnection(element)) return 0;

        float power = 0;

        var storages = connections.OfType<IStorage>();
        var usedWires = new HashSet<IWire>();
        foreach (var storage in storages)
        {
            //TODO: Fix this dumb shit
            var path = PathFinder.FindBestPath(storage, element, usedWires);
            if (path.Count == 0) continue;
            var wires = path.OfType<IWire>();
            foreach (var wire in wires) usedWires.Add(wire);
            power += PowerFlow.CalculatePower(storage.Count(Consts.storagePowerKey), path);
        }

        return power;
    }
}