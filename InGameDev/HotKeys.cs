using TheElectrician.Objects.Mono;

namespace TheElectrician.InGameDev;

internal static class HotKeys
{
    public static void Update()
    {
        if (!m_localPlayer) return;
        if (!Terminal.m_cheat) return;
        if (Input.GetKeyDown(KeyCode.Keypad0)) ClearStorage();
        if (Input.GetKeyDown(KeyCode.Keypad1)) FillStorage();
    }

    private static void FillStorage()
    {
        var storage = GetHoveringStorage();
        storage?.Add(Consts.storagePowerKey, storage.FreeSpace());
    }

    [CanBeNull]
    private static IStorage GetHoveringStorage() => GetHoveringEO() as IStorage;

    [CanBeNull]
    private static IElectricObject GetHoveringEO()
    {
        var hoverObject = m_localPlayer.GetHoverObject()?.GetComponentInParent<ZNetView>()?.gameObject;
        if (!hoverObject) return null;

        var mono = ElectricMono.GetAll().Find(x => x.gameObject == hoverObject);
        if (mono is null) return null;
        return Library.GetObject(mono.GetId());
    }

    private static void ClearStorage()
    {
        var storage = GetHoveringStorage();
        storage?.Clear();
    }
}