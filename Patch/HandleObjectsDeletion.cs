namespace TheElectrician.Patch;

[HarmonyWrapSafe]
//TODO: ZDOMan.DestroyZDO
[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Destroy), typeof(GameObject))]
file class HandleObjectsDeletion
{
    [HarmonyPrefix] [UsedImplicitly]
    public static void Patch(GameObject go)
    {
        if (!go) return;
        var netView = go.GetComponent<ZNetView>();
        if (!netView) return;
        EOLifeHandler.DestroyEO(netView.GetZDO());
    }
}