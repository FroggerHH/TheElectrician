using TheElectrician.Objects.Mono;

namespace TheElectrician.Patch;

[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))] [HarmonyWrapSafe]
file static class FinishPieces
{
    [UsedImplicitly] [HarmonyPostfix]
    private static void Postfix(ZNetScene __instance)
    {
        var generatorPiece = piece("TE_coalGenerator");
        var generatorPieceWN = wearNTear("TE_coalGenerator");
        var woodPlaceEffect = piece("woodwall").m_placeEffect;
        var woodwallWN = wearNTear("woodwall");
        generatorPiece.m_placeEffect = woodPlaceEffect;
        generatorPieceWN.m_hitEffect = woodwallWN.m_hitEffect;
        FixShaders(generatorPiece);
        var smoke = generatorPiece.GetComponentInChildren<SmokeSpawner>(true);
        smoke.m_smokePrefab = prefab("charcoal_kiln").GetComponentInChildren<SmokeSpawner>(true).m_smokePrefab;
        generatorPiece.gameObject.GetOrAddComponent<MonoGenerator>();
    }

    private static void FixShaders(Component obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var ren in renderers)
            ren.material.shader = Shader.Find(ren.material.shader.name);
    }

    private static ItemDrop item(string name) { return prefab(name)?.GetComponent<ItemDrop>(); }
    private static GameObject prefab(string name) { return ZNetScene.instance?.GetPrefab(name); }

    private static Piece piece(string name) { return prefab(name)?.GetComponent<Piece>(); }

    private static WearNTear wearNTear(string name) { return prefab(name)?.GetComponent<WearNTear>(); }
}