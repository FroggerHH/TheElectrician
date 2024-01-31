namespace TheElectrician;

public static class Consts
{
    public static readonly float tickTime = 0.5f;
    public static readonly string electricObjectIdKey = "ElectricObjectId";

    public static readonly int defaultStorageMaxConnections = 1;
    public static readonly string currentStorageKey = "CurrentStorage";
    public static readonly string storagePowerKey = "GetPowerStored";
    public static readonly string capacityKey = "Capacity";
    public static readonly string storageKey = "Storage";

    public static readonly string powerPerTickKey = "PowerPerTick";
    public static readonly string fuelPerTickKey = "FuelPerTick";
    public static readonly string maxFuelKey = "MaxFuel";
    public static readonly string fuelItemKey = "FuelItem";

    public static readonly int defaultWireMaxConnections = 3;
    public static readonly float wireMaxFalloffByDistance = 0.6f;
    public static readonly string connectionsKey = "Connections";
    public static readonly string wireConductivityKey = "WireConductivity";
    public static readonly string wireManagedObjectIdKey = "WireManagedObjectId";
}