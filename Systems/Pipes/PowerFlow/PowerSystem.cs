namespace TheElectrician.Systems.PowerFlow;

public class PowerSystem : IEquatable<PowerSystem>
{
    private HashSet<IWireConnectable> connections = [];

    public float GetPowerStored() { return connections.OfType<IStorage>().Sum(x => x.GetPower()); }

    public override string ToString() { return $"Connections: {connections.GetString()}, Power: {GetPowerStored()}"; }

    public bool ContainsConnection(IWireConnectable element) { return connections.Contains(element); }

    public HashSet<IWireConnectable> GetConnections() { return connections; }

    public void SetConnections(HashSet<IWireConnectable> connections) { this.connections = connections; }

    // public float GetPossiblePowerInElement(IWireConnectable element)
    // {
    //     if (element is null) return 0;
    //     if (!ContainsConnection(element)) return 0;
    //
    //     float power = 0;
    //
    //     var localCashedConnections = new Dictionary<IWire, float>();
    //     var storages = GetStorages().OrderByDescending(x => x.GetPower());
    //     foreach (var storage in storages)
    //     {
    //         var path = PathFinder.FindBestPath(storage, element, localCashedConnections, true);
    //         if (path.Count == 0)
    //             continue;
    //
    //         var initialPower = storage.GetPower();
    //         var calculatePower = PowerFlow.CalculatePower(initialPower, path, localCashedConnections, true);
    //         power += calculatePower;
    //         PathFinder.ApplyPathToVirtualConductivityCache(path, initialPower, localCashedConnections);
    //     }
    //
    //     return Clamp(power, 0, element.GetConductivity());
    // }

    public List<IStorage> GetStorages() => connections.OfType<IStorage>().Where(x => x is not IConsumer).ToList();

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