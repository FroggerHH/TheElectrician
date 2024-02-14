using System.Diagnostics.CodeAnalysis;
using TheElectrician.Extensions;

namespace TheElectrician.Systems.PowerFlow;

[PublicAPI]
public static class PowerFlow
{
    private static HashSet<PowerSystem> powerSystems = [];

    public static void Start()
    {
        Debug("PowerFlow: Starting update");
        GetPlugin().StartCoroutine(UpdateEnumerator());
    }

    public static void Destroy()
    {
        Debug("PowerFlow: Stopping update");
        GetPlugin().StopCoroutine(UpdateEnumerator());
    }

    [SuppressMessage("ReSharper", "FunctionRecursiveOnAllPaths")]
    private static IEnumerator UpdateEnumerator()
    {
        yield return new WaitForSeconds(TheConfig.PowerTickTime);
        Update();
        GetPlugin().StartCoroutine(UpdateEnumerator());
    }

    private static void Update()
    {
        PathFinder.ClearCache();
        FormPowerSystems();
        TransportPowerToStorages();
    }

    private static void TransportPowerToStorages()
    {
        //TODO: The maximum distance between the wires (you cannot connect two wires from different sides of the world)
        //TODO: Checking the wire block (the cable cannot pass through obstacles)

        foreach (var powerSys in powerSystems)
        {
            var storages = powerSys.GetStorages()
                .Where(x => x is not IGenerator && x is not IConsumer
                                                && x.CanAccept(Consts.storagePowerKey) && !x.IsFull())
                .OrderBy(x => x.GetPower()).ToList();
            var generators = powerSys.GetConnections().OfType<IGenerator>()
                .Where(x => x.GetPower() > 0).OrderByDescending(x => x.GetPower())
                .ToList();

            if (storages.Count == 0 || generators.Count == 0) continue;
            foreach (var storage in storages)
            {
                // Debug($"storage {storage.GetObjectString()}");
                if (storage.IsFull()) continue;
                foreach (var gen in generators)
                {
                    var toAdd = Min(storage.FreeSpace(), gen.GetPower());
                    if (toAdd == 0) continue;

                    var path = PathFinder.FindBestPath(storage, gen);
                    if (path.Count == 0) continue;
                    var calculatedPower = CalculatePower(toAdd, path);
                    gen.TransferTo(storage, Consts.storagePowerKey, calculatedPower);
                    PathFinder.ApplyPath(path, toAdd);

                    return;
                }
            }
        }
    }

    public static float CalculatePower(float initialPower, HashSet<IWireConnectable> path,
        Dictionary<IWire, float> virtualWiresConductivityCache = null, bool checkVirtualAndRealCache = false)
    {
        var resultPower = initialPower;

        var conductivityCacheToCheck = PathFinder.wiresConductivityCache;
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


        foreach (var point in path)
        {
            resultPower = Clamp(resultPower, 0, point.GetConductivity());
            if (point is IWire wire && conductivityCacheToCheck.TryGetValue(wire, out var powerUsed))
                resultPower = Clamp(resultPower, 0, wire.GetConductivity() - powerUsed);

            resultPower *= 1 - (point.GetPowerLoss() / 100);
            if (resultPower <= float.Epsilon) break;
        }

        // Debug($"Calculated {initialPower}->{resultPower} power. "
        //       + $"PathConductivity: {path.Select(x => x.GetConductivity()).GetString()}");
        return resultPower;
    }

    private static void FormPowerSystems()
    {
        powerSystems.Clear();
        PowerSystem currentPowerSys;
        var allStorages = Library.GetAllObjects<Storage>();
        foreach (var storage in allStorages)
        {
            currentPowerSys = new PowerSystem();
            powerSystems.Add(currentPowerSys);

            GoThroughConnections([storage]);
        }

        var connectedPowerSystems = new HashSet<PowerSystem>(powerSystems);
        foreach (var powerSys in powerSystems)
        {
            var connected = powerSystems.FirstOrDefault(x =>
                x.GetConnections().Any(x1 => powerSys.GetConnections().Contains(x1)));
            if (connected != null && !powerSys.Equals(connected))
            {
                powerSys.SetConnections(powerSys.GetConnections().Union(connected.GetConnections()).ToHashSet());
                connectedPowerSystems.RemoveWhere(x => x.Equals(connected));
            }
        }

        powerSystems = connectedPowerSystems;

        void GoThroughConnections(HashSet<IPipeableConnectable> connections)
        {
            foreach (var electricObject in connections)
            {
                if (currentPowerSys.GetConnections().Contains(electricObject)) continue;
                currentPowerSys.GetConnections().Add(electricObject as IWireConnectable);

                GoThroughConnections(electricObject.GetConnections());
            }
        }
    }

    public static PowerSystem GetPowerSystem(IWireConnectable element)
    {
        return powerSystems.FirstOrDefault(x => x.ContainsConnection(element));
    }

    public static bool ConsumePower(IConsumer consumer, float amount)
    {
        var powerSys = GetPowerSystem(consumer as IWireConnectable);
        if (powerSys == null) return false;

        return powerSys.ConsumePower(consumer, amount);
    }
}