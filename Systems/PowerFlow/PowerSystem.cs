using TheElectrician.Extensions;

namespace TheElectrician.Systems.PowerFlow;

public class PowerSystem : IEquatable<PowerSystem>
{
    private HashSet<IWireConnectable> connections = [];

    public float GetPowerStored() { return connections.OfType<IStorage>().Sum(x => x.GetPower()); }

    public override string ToString() { return $"Connections: {connections.GetString()}, Power: {GetPowerStored()}"; }

    public bool ContainsConnection(IWireConnectable element) { return connections.Contains(element); }

    public HashSet<IWireConnectable> GetConnections() { return connections; }

    public void SetConnections(HashSet<IWireConnectable> connections) { this.connections = connections; }

    public float GetPossiblePowerInElement(IWireConnectable element)
    {
        if (element is null) return 0;
        if (!ContainsConnection(element)) return 0;

        float power = 0;

        var storages = GetStorages().OrderByDescending(x => x.GetPower());
        var usedWires = new HashSet<IWire>();
        foreach (var storage in storages)
        {
            var path = PathFinder.FindBestPath(storage, element, usedWires);
            if (path.Count == 0) continue;
            var wires = path.OfType<IWire>().ToList();
            foreach (var wire in wires) usedWires.Add(wire);
            power += PowerFlow.CalculatePower(storage.GetPower(), path);
        }

        return Clamp(power, 0, element.GetConductivity());
    }

    public List<IStorage> GetStorages() { return connections.OfType<IStorage>().ToList(); }

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

    public bool Equals(PowerSystem other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return connections.SetEquals(other.connections);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((PowerSystem)obj);
    }

    public override int GetHashCode() => connections != null ? connections.GetHashCode() : 0;
}