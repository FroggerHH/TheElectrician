using TheElectrician.Objects.Mono;

namespace TheElectrician.Patch;

[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))] [HarmonyWrapSafe]
file static class FinishPieces
{
    private static EffectList woodPlaceEffect;
    private static WearNTear woodwallWN;
    private static WearNTear smelterWN;

    [UsedImplicitly] [HarmonyPostfix]
    private static void Postfix(ZNetScene __instance)
    {
        woodPlaceEffect = piece("woodwall").m_placeEffect;
        woodwallWN = wearNTear("woodwall");
        smelterWN = wearNTear("smelter");
        TE_coalGenerator();
        TE_woodStorage();
        TE_woodWire();
        TE_stoneFurnace();
    }

    private static void TE_woodWire()
    {
        var wirePiece = piece("TE_woodWire");
        var wirePieceWN = wearNTear("TE_woodWire");
        wirePiece.m_placeEffect = woodPlaceEffect;
        wirePieceWN.m_hitEffect = woodwallWN.m_hitEffect;
        FixShaders(wirePiece);
        wirePiece.gameObject.GetOrAddComponent<MonoWire>();
    }

    private static void TE_woodStorage()
    {
        var storagePiece = piece("TE_woodenStorage");
        var storagePieceWN = wearNTear("TE_woodenStorage");
        storagePiece.m_placeEffect = woodPlaceEffect;
        storagePieceWN.m_hitEffect = woodwallWN.m_hitEffect;
        FixShaders(storagePiece);
        storagePiece.gameObject.GetOrAddComponent<MonoStorage>();
    }

    private static void TE_coalGenerator()
    {
        var generatorPiece = piece("TE_coalGenerator");
        var generatorPieceWN = wearNTear("TE_coalGenerator");
        generatorPiece.m_placeEffect = piece("smelter").m_placeEffect;
        generatorPieceWN.m_hitEffect = smelterWN.m_hitEffect;
        FixShaders(generatorPiece);
        var smoke = generatorPiece.GetComponentInChildren<SmokeSpawner>(true);
        smoke.m_smokePrefab = prefab("charcoal_kiln").GetComponentInChildren<SmokeSpawner>(true).m_smokePrefab;
        generatorPiece.gameObject.GetOrAddComponent<MonoGenerator>();
    }

    private static void TE_stoneFurnace()
    {
        var stoneFurnacePiece = piece("TE_stoneFurnace");
        var stoneFurnaceWN = wearNTear("TE_stoneFurnace");
        stoneFurnacePiece.m_placeEffect = piece("smelter").m_placeEffect;
        stoneFurnaceWN.m_hitEffect = smelterWN.m_hitEffect;
        FixShaders(stoneFurnacePiece);
        var smoke = stoneFurnacePiece.GetComponentInChildren<SmokeSpawner>(true);
        smoke.m_smokePrefab = prefab("charcoal_kiln").GetComponentInChildren<SmokeSpawner>(true).m_smokePrefab;
        var guidePoint = stoneFurnacePiece.GetComponentInChildren<GuidePoint>(true);
        guidePoint.m_ravenPrefab = prefab("piece_workbench").GetComponentInChildren<GuidePoint>(true).m_ravenPrefab;
        stoneFurnacePiece.gameObject.GetOrAddComponent<MonoFurnace>();
    }

    private static void FixShaders(Component obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>(true);
        foreach (var ren in renderers) ren.material.shader = Shader.Find(ren.material.shader.name);
    }

    private static ItemDrop item(string name) { return prefab(name)?.GetComponent<ItemDrop>(); }
    private static GameObject prefab(string name) { return ZNetScene.instance?.GetPrefab(name); }

    private static Piece piece(string name) { return prefab(name)?.GetComponent<Piece>(); }

    private static WearNTear wearNTear(string name) { return prefab(name)?.GetComponent<WearNTear>(); }
}