namespace TheElectrician.Extensions;

[PublicAPI]
public static class StorageExtension
{
    public static float GetPower(this IStorage storage) => storage.Count(Consts.storagePowerKey);
    public static bool AcceptPower(this IStorage storage) => storage.CanAccept(Consts.storagePowerKey);
}