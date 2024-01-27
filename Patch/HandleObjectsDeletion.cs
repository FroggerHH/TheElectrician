namespace TheElectrician.Patch;

[HarmonyPatch(typeof(WearNTear), nameof(WearNTear.RPC_Remove))] [HarmonyWrapSafe]
file class HandleObjectsDeletion
{
    [HarmonyPrefix] [UsedImplicitly]
    public static void Remove(WearNTear __instance)
    {
        if (!__instance) return;
        Debug(
            $"Piece destroyed: {__instance}, m_nview: {__instance.m_nview?.ToString() ?? "null"}, zdo: {__instance.m_nview?.GetZDO()?.ToString() ?? "null"}");
        Library.RemoveObject(Library.GetObject(__instance.m_nview.GetZDO()));
    }
}