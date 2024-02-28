using System.Diagnostics.CodeAnalysis;

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
        //TODO: Checking the wire block (the cable cannot pass through obstacles)

        foreach (var powerSys in powerSystems)
        {
            var allStorages = powerSys.GetConnections().OfType<IStorage>()
                .Where(x => x is not IGenerator && x.AcceptPower())
                .OrderBy(x => x.GetPower()).ToList();
            var storages = allStorages.FindAll(x => x is not IConsumer);
            var consumers = allStorages.OfType<IConsumer>().ToList();
            var generators = powerSys.GetConnections().OfType<IGenerator>()
                .Where(x => !x.IsEmpty(true)).OrderByDescending(x => x.GetPower())
                .ToList();

            if (storages.Count == 0) continue;
            foreach (var consumer in consumers)
            {
                if (consumer.IsFull(true)) continue;

                foreach (var st in storages)
                {
                    var initialPowerToAdd = Min(consumer.FreeSpace(true), st.GetPower());
                    if (initialPowerToAdd <= float.Epsilon) continue;

                    var path = PathFinder.FindBestPath(consumer, st);
                    if (path.Count == 0) continue;
                    var calculatedPower = CalculatePower(initialPowerToAdd, path);
                    if (calculatedPower <= float.Epsilon) continue;
                    st.TransferTo(consumer, Consts.storagePowerKey, calculatedPower);
                    PathFinder.ApplyPath(path, initialPowerToAdd);
                }
            }

            if (generators.Count == 0) continue;
            foreach (var storage in storages)
            {
                if (storage.IsFull(true))
                {
                    continue;
                }

                foreach (var gen in generators)
                {
                    var initialPowerToAdd = Min(storage.FreeSpace(true), gen.GetPower());
                    if (initialPowerToAdd <= float.Epsilon) continue;

                    var path = PathFinder.FindBestPath(storage, gen);
                    if (path.Count == 0) continue;

                    var calculatedPower = CalculatePower(initialPowerToAdd, path);
                    if (calculatedPower <= float.Epsilon) continue;

                    gen.TransferTo(storage, Consts.storagePowerKey, calculatedPower);
                    PathFinder.ApplyPath(path, initialPowerToAdd);
                }
            }
        }
    }

    public static float CalculatePower(float initialPower, HashSet<IWireConnectable> path)
    {
        var resultPower = initialPower;

        foreach (var point in path)
        {
            resultPower = Clamp(resultPower, 0, point.GetConductivity());
            if (point is IWire wire && PathFinder.wiresConductivityCache.TryGetValue(wire, out var powerUsed))
                resultPower = Clamp(resultPower, 0, wire.GetConductivity() - powerUsed);

            resultPower *= 1 - point.GetPowerLoss() / 100;
            if (resultPower <= float.Epsilon) return 0;
        }

        // Debug($"Calculated {initialPower}->{resultPower} power");
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

        var connectedPowerSystems = new List<PowerSystem>();
        foreach (var powerSys in powerSystems)
        {
            var connected =
                powerSystems.FirstOrDefault(x => x.GetConnections().Any(x1 => powerSys.GetConnections().Contains(x1)));
            if (connected != null && !connectedPowerSystems.Exists(x => x.Equals(connected)))
            {
                powerSys.SetConnections(powerSys.GetConnections().Union(connected.GetConnections()).ToHashSet());
                connectedPowerSystems.Add(powerSys);
            }
        }

        powerSystems = connectedPowerSystems.ToHashSet();

        void GoThroughConnections(HashSet<IPipeConnectable> connections)
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
}