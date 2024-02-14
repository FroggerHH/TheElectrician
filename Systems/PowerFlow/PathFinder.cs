namespace TheElectrician.Systems.PowerFlow;

internal static class PathFinder
{
    internal static readonly Dictionary<IWire, float> wiresConductivityCache = [];

    public static HashSet<IWireConnectable> FindBestPath(IWireConnectable start, IWireConnectable end,
        Dictionary<IWire, float> virtualWiresConductivityCache = null, bool checkVirtualAndRealCache = false)
    {
        Dictionary<IWireConnectable, float> distances = [];
        Dictionary<IWireConnectable, IWireConnectable> previousNodes = [];
        HashSet<IWireConnectable> unvisitedNodes = [];

        var conductivityCacheToCheck = wiresConductivityCache;
        if (virtualWiresConductivityCache != null)
            if (checkVirtualAndRealCache)
            {
                foreach (var pair in virtualWiresConductivityCache)
                    if (!conductivityCacheToCheck.ContainsKey(pair.Key))
                        conductivityCacheToCheck.Add(pair.Key, pair.Value);
                    else if (conductivityCacheToCheck[pair.Key] < pair.Value)
                        conductivityCacheToCheck[pair.Key] = pair.Value;
            } else
                conductivityCacheToCheck = virtualWiresConductivityCache;


        foreach (var node in start.GetConnections().OfType<IWireConnectable>())
        {
            //If a power equal to or exceeding the conductivity of this wire has already passed through
            //this wire during this tick, then it can no longer be used in this tick. ⬇
            if (IsWireTooBusy(node, wiresConductivityCache)) continue;

            distances[node] = GetWeight(node);
            previousNodes[node] = start;
            unvisitedNodes.Add(node);
        }

        distances[start] = 0;

        while (unvisitedNodes.Count > 0)
        {
            var currentNode = unvisitedNodes.OrderBy(node => distances[node]).First();
            unvisitedNodes.Remove(currentNode);

            foreach (var neighbor in currentNode.GetConnections().OfType<IWireConnectable>())
            {
                if (IsWireTooBusy(neighbor, conductivityCacheToCheck)) continue;

                var alt = distances[currentNode] + GetWeight(neighbor);
                if (!distances.ContainsKey(neighbor) || alt < distances[neighbor])
                {
                    distances[neighbor] = alt;
                    previousNodes[neighbor] = currentNode;
                    unvisitedNodes.Add(neighbor);
                }
            }
        }

        HashSet<IWireConnectable> path = [];
        var current = end;
        while (previousNodes.ContainsKey(current))
        {
            path.Add(current);
            current = previousNodes[current];
        }

        path.Add(start);
        if (path.Count <= 1) return [];

        return path.Reverse().ToHashSet();
    }

    private static bool IsWireTooBusy(IWireConnectable node, Dictionary<IWire, float> conductivityCache)
    {
        return node is IWire wire && conductivityCache.TryGetValue(wire, out var powerPassed)
                                  && powerPassed + Consts.minPower >= node.GetConductivity();
    }


    private static float GetWeight(IWireConnectable node) => (node is IWire wire) ? wire.GetConductivity() : 0.001f;

    public static void ClearCache()
    {
        wiresConductivityCache.Clear();
        Debug($"--- Cleared wires cache ---");
    }

    public static void ApplyPathToVirtualConductivityCache(IEnumerable<IWireConnectable> path_, float powerSignal,
        Dictionary<IWire, float> virtualWiresConductivityCache)
    {
        var path = path_.ToList();
        var wires = path.OfType<IWire>().ToList();
        foreach (var wire in wires)
        {
            var pathFromStartToWire = path.GetRange(0, path.IndexOf(wire) + 1).ToHashSet();
            var calculatePower = PowerFlow.CalculatePower(powerSignal, pathFromStartToWire);
            if (virtualWiresConductivityCache.ContainsKey(wire)) virtualWiresConductivityCache[wire] += calculatePower;
            else virtualWiresConductivityCache.Add(wire, calculatePower);
        }
    }

    public static void ApplyPath(IEnumerable<IWireConnectable> path_, float powerSignal)
    {
        var path = path_.ToList();
        var wires = path.OfType<IWire>().ToList();

        foreach (var wire in wires)
        {
            var pathFromStartToWire = path.GetRange(0, path.IndexOf(wire) + 1).ToHashSet();
            var calculatePower = PowerFlow.CalculatePower(powerSignal, pathFromStartToWire);
            // wiresConductivityCache[wire] = calculatePower;
            if (wiresConductivityCache.ContainsKey(wire)) wiresConductivityCache[wire] += calculatePower;
            else wiresConductivityCache.Add(wire, calculatePower);
        }
    }
}