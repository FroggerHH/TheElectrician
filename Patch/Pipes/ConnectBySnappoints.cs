using TheElectrician.Settings;
using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Patch.Pipes;

[HarmonyPatch, HarmonyWrapSafe]
file static class ConnectBySnappoints
{
    [HarmonyPatch(typeof(Player), nameof(Player.UpdatePlacementGhost))]
    static void Postfix(Player __instance)
    {
        if (__instance != m_localPlayer || __instance == null) return;
        if (__instance.m_placementStatus != PlacementStatus.Valid) return;
        var ghost = __instance.m_placementGhost?.GetComponent<Piece>();
        if (ghost is null) return;
        var hovering = __instance.m_hoveringPiece;
        var container = hovering?.GetComponent<Container>();
        if (hovering is null) return;
        var ghostSettings = Library.GetSettings(ghost.GetPrefabName());
        if (ghostSettings is null || ghostSettings is not IItemPipeSettings) return;
        var targetSettings = Library.GetSettings(hovering.GetPrefabName());
        if ((targetSettings is null || targetSettings is not IItemPipeConnectableSettings) && !container)
        {
            __instance.m_placementStatus = PlacementStatus.Invalid;
            __instance.SetPlacementGhostValid(false);
            return;
        }

        GetConnections(hovering.transform, out var targetAllConnections, out _);
        GetConnections(ghost.transform, out var ghostAllConnections, out var activeConnections);

        var nearestTargetSnappoint = targetAllConnections.Nearest(ghost.transform.position);
        var currentSnappoint = ghostAllConnections.Nearest(nearestTargetSnappoint.position);
        Debug($"nearestTargetSnappoint = {nearestTargetSnappoint.name}, currentSnappoint = {currentSnappoint.name}");

        if (!activeConnections.Contains(currentSnappoint)
            || Vector3.Distance(nearestTargetSnappoint.position, currentSnappoint.position) > 0.6f)
        {
            __instance.m_placementStatus = PlacementStatus.Invalid;
            __instance.SetPlacementGhostValid(false);
            return;
        }
    }

    private static void GetConnections(Transform piece, out List<Transform> all, out List<Transform> active)
    {
        all = [];
        active = [];
        if (piece is null) return;

        var connectionsHolder = piece.FindChildByName("PipeConnections");
        if (connectionsHolder is null) return;
        foreach (Transform _child in connectionsHolder)
        {
            all.Add(_child);
            if (_child.name.Contains("(Active Connection)")) active.Add(_child);
        }
    }
}