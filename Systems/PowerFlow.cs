using TheElectrician.Models;

namespace TheElectrician.Systems;

public static class PowerFlow
{
    private static float cashedPowerInSystem = -1;

    public static void Update()
    {
        HashSet<IWire> visitedWires = new HashSet<IWire>();
        HashSet<IStorage> visitedStorages = new HashSet<IStorage>();

        Queue<IElectricObject> queue = new Queue<IElectricObject>(Library.GetAllObjects<IStorage>());

        while (queue.Count > 0)
        {
            var electricObject = queue.Dequeue();

            if (electricObject is IWire wire)
            {
                if (visitedWires.Add(wire))
                    foreach (var connection in wire.GetConnections())
                        queue.Enqueue(connection);
            } else if (electricObject is IStorage storage)
                if (visitedStorages.Add(storage))
                    foreach (var connectedWire in storage.GetConnectedWires())
                        queue.Enqueue(connectedWire);
        }

        var generators = visitedStorages.OfType<IGenerator>().Where(x => x.Count(Consts.storagePowerKey) > 0).ToList();
        var storages = visitedStorages.Where(x => x is not IGenerator && !x.IsFull())
            .OrderBy(x => x.Count(Consts.storagePowerKey)).ToList();

        cashedPowerInSystem = visitedStorages.Sum(x => x.Count(Consts.storagePowerKey));
        Debug($"PowerFlow, visitedStorages: {visitedStorages.GetString()}, powerInSystem: {cashedPowerInSystem}");
        if (storages.Count > 0)
        {
            foreach (var storage in storages)
            {
                foreach (var generator in generators)
                {
                    var toAdd = Min(storage.FreeSpace(), generator.Count(Consts.storagePowerKey));
                    if (storage.CanAdd(toAdd) && generator.CanRemove(Consts.storagePowerKey, toAdd))
                    {
                        storage.Add(Consts.storagePowerKey, toAdd);
                        generator.Remove(Consts.storagePowerKey, toAdd);
                    }
                }
            }
        }
    }

    public static float GetPowerInSystem(IWire wire) { return cashedPowerInSystem; }
}