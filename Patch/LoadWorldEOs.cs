namespace TheElectrician.Patch;

[HarmonyPatch(typeof(ZDOMan))] [HarmonyWrapSafe] [UsedImplicitly]
file static class LoadWorldEOs
{
    [HarmonyPatch(nameof(ZDOMan.Load))] [HarmonyPostfix]
    private static void Load() { EOLifeHandler.Load(); }
}