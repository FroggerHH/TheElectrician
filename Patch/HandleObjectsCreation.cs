using TheElectrician.Objects.Mono;

namespace TheElectrician.Patch;

[HarmonyPatch]
file class HandleObjectsCreation
{
    [HarmonyPatch(typeof(Piece), nameof(Piece.Awake))] [HarmonyWrapSafe]
    [HarmonyPostfix] [UsedImplicitly]
    public static void PieceAwake(Piece __instance)
    {
        if (!__instance || !__instance.m_nview || !__instance.m_nview.IsValid()) return;
        if (!Library.IsEO(__instance)) return;

        EOLifeHandler.CreateNewEO(__instance.m_nview.GetZDO(), out _);
        var electricMono = __instance.GetComponent<ElectricMono>();
        if (electricMono)
        {
            electricMono.SetUp();
            electricMono.Load();
        }
    }
}