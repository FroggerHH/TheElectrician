namespace TheElectrician.Patch;

[HarmonyPatch(typeof(Game))] [HarmonyWrapSafe] [UsedImplicitly]
file static class GameStartEnd
{
    [HarmonyPatch(nameof(Game.Start))] [HarmonyPostfix]
    private static void Start()
    {
        Library.Clear();
        Library.AddObjectsFromWorld();
        Updater.Start();
    }

    [HarmonyPatch(nameof(Game.OnDestroy))] [HarmonyPostfix]
    private static void End()
    {
        Library.Clear();
        Updater.Destroy();
    }
}