namespace TheElectrician.Patch;

[HarmonyPatch(typeof(Game))] [HarmonyWrapSafe] [UsedImplicitly]
file static class GameStartEnd
{
    [HarmonyPatch(nameof(Game.Start))] [HarmonyPostfix]
    private static void Start()
    {
        EOLifeHandler.Clear();
        Updater.Start();
        PowerFlow.Start();
    }

    [HarmonyPatch(nameof(Game.OnDestroy))] [HarmonyPostfix]
    private static void End()
    {
        EOLifeHandler.Clear();
        Updater.Destroy();
        PowerFlow.Destroy();
    }
}