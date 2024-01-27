namespace TheElectrician.Patch;

[HarmonyPatch]
file class HandleObjectsCreation
{
    [HarmonyPatch(typeof(Piece), nameof(Piece.Awake))] [HarmonyWrapSafe]
    [HarmonyPostfix] [UsedImplicitly]
    public static void PieceAwake(Piece __instance)
    {
        if (!__instance || !__instance.m_nview || !__instance.m_nview.IsValid()) return;

        Library.SpawnObject(__instance.m_nview.GetZDO());
    }

    [HarmonyPatch(typeof(Game), nameof(Game.SpawnPlayer))] [HarmonyWrapSafe]
    [HarmonyPostfix] [UsedImplicitly]
    public static void Player_Spawn() { Library.AddObjectsFromWorld(); }
}

//TODO: Add Wire object