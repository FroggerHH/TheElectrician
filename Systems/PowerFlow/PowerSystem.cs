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

        var storages = GetStorages();
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

    private List<IStorage> GetStorages() { return connections.OfType<IStorage>().ToList(); }

    public bool ConsumePower(IConsumer consumer, float amount)
    {
        if (consumer is null) return false;

        var asWireConn = consumer as IWireConnectable;
        if (!ContainsConnection(asWireConn)) return false;

        var storages = GetStorages();
        var usedWires = new HashSet<IWire>();
        var storagesWithPower = new Dictionary<IStorage, float>();

        foreach (var storage in storages)
        {
            var path = PathFinder.FindBestPath(storage, asWireConn, usedWires);
            if (path.Count == 0) continue;
            var wires = path.OfType<IWire>();
            foreach (var wire in wires) usedWires.Add(wire);

            var power = PowerFlow.CalculatePower(storage.Count(Consts.storagePowerKey), path);
            if (power <= float.Epsilon) continue;
            storagesWithPower.Add(storage, power);
        }
        
        if (storagesWithPower.Count == 0) return false;
        if (storagesWithPower.Sum(x => x.Value) < amount) return false;
        storagesWithPower = storagesWithPower.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        var consumedPower = 0f;
        foreach (var pair in storagesWithPower)
        {
            var storage = pair.Key;
            var powerStored = pair.Value;

            if (consumedPower >= amount) break;

            var toConsume = Min(amount - consumedPower, powerStored);
            consumedPower += toConsume;
            storage.Remove(Consts.storagePowerKey, toConsume);
        }

        return true;
    }
}