using TheElectrician.Models;
using TheElectrician.Objects;

namespace TheElectrician.Systems;

public static class PowerFlow
{
    private static HashSet<PowerSystem> powerSystems = new();
    private static PowerSystem currentPowerSys;

    public static void Update()
    {
        FormPowerSystems();
        TransportPowerToStorages();
    }

    private static void TransportPowerToStorages()
    {
        foreach (var powerSys in powerSystems)
        {
            var storages = powerSys.GetConnections().OfType<Storage>().Where(x => x is not IGenerator && !x.IsFull())
                .OrderBy(x => x.Count(Consts.storagePowerKey)).ToList();
            var generators = powerSys.GetConnections().OfType<IGenerator>()
                .Where(x => x.Count(Consts.storagePowerKey) > 0).ToList();
            foreach (var storage in storages)
            foreach (var gen in generators)
            {
                var toAdd = Min(storage.FreeSpace(), gen.Count(Consts.storagePowerKey));
                gen.TransferTo(storage, Consts.storagePowerKey, toAdd);
            }
        }
    }

    private static void FormPowerSystems()
    {
        powerSystems.Clear();
        currentPowerSys = null;
        var allStorages = Library.GetAllObjects<Storage>();
        // Debug(
        //     $"FormPowerSystems allStorages: {allStorages.Select(x => x?.ToString() ?? "null").GetString() ?? "null"}");
        foreach (var storage in allStorages)
        {
            currentPowerSys = new PowerSystem();
            powerSystems.Add(currentPowerSys);

            GoThroughConnections(new HashSet<IWireConnectable> { storage });
            currentPowerSys = null;
        }

        var connectedPowerSystems = new HashSet<PowerSystem>(powerSystems);
        foreach (var powerSys in powerSystems)
        {
            var connected =
                powerSystems.FirstOrDefault(x => x.GetConnections().Any(x => powerSys.GetConnections().Contains(x)));
            if (connected != null && powerSys != connected)
            {
                powerSys.SetConnections(powerSys.GetConnections().Union(connected.GetConnections()).ToHashSet());
                connectedPowerSystems.Remove(connected);
            }
        }

        powerSystems = connectedPowerSystems;
    }

    private static void GoThroughConnections(HashSet<IWireConnectable> connections)
    {
        foreach (var electricObject in connections)
        {
            if (currentPowerSys.GetConnections().Contains(electricObject)) continue;
            currentPowerSys.GetConnections().Add(electricObject);

            GoThroughConnections(electricObject.GetConnections());
        }
    }

    public static float GetPowerInSystem(IWireConnectable element)
    {
        return GetPowerSystem(element)?.GetPowerStored() ?? -1;
    }

    public static PowerSystem GetPowerSystem(IWireConnectable element)
    {
        return powerSystems.FirstOrDefault(x => x.ContainsConnection(element));
    }
}