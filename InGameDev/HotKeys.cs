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
        storage?.Add(Consts.storagePowerKey, storage.FreeSpace(true));
    }

    [CanBeNull]
    public static IStorage GetHoveringStorage() => m_localPlayer.GetHoveringEO() as IStorage;

    public static ILevelable GetHoveringLevelable() => m_localPlayer.GetHoveringEO() as ILevelable;

    private static void ClearStorage()
    {
        var storage = GetHoveringStorage();
        storage?.Clear();
    }
}