namespace TheElectrician.Patch;

[HarmonyPatch(typeof(Game))] [HarmonyWrapSafe] [UsedImplicitly]
file static class GameStartEnd
{
    [HarmonyPatch(nameof(Game.Start))] [HarmonyPostfix]
    private static async void Start()
    {
        EOLifeHandler.Clear();
        await EOLifeHandler.Load();
        Updater.Start();
    }

    [HarmonyPatch(nameof(Game.OnDestroy))] [HarmonyPostfix]
    private static void End()
    {
        EOLifeHandler.Clear();
        Updater.Destroy();
    }
}