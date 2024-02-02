using TheElectrician.Models;

namespace TheElectrician.Systems.PowerFlow;

internal static class PathFinder
{
    public static HashSet<IWireConnectable> FindBestPath(IWireConnectable start, IWireConnectable end)
    {
        Dictionary<IWireConnectable, float> distances = new();
        Dictionary<IWireConnectable, IWireConnectable> previousNodes = new();
        HashSet<IWireConnectable> unvisitedNodes = [];

        foreach (var node in start.GetConnections())
        {
            distances[node] = GetWeight(node);
            previousNodes[node] = start;
            unvisitedNodes.Add(node);
        }

        distances[start] = 0;

        while (unvisitedNodes.Count > 0)
        {
            var currentNode = unvisitedNodes.OrderBy(node => distances[node]).First();
            unvisitedNodes.Remove(currentNode);

            foreach (var neighbor in currentNode.GetConnections())
            {
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
        IWireConnectable current = end;
        while (previousNodes.ContainsKey(current))
        {
            path.Add(current);
            current = previousNodes[current];
        }

        path.Add(start);

        return path.Reverse().ToHashSet();
    }

    private static float GetWeight(IWireConnectable node) => (node is IWire wire) ? wire.GetConductivity() : 0.001f;
}