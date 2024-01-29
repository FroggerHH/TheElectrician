using TheElectrician.Objects.Mono;

namespace TheElectrician.Patch;

[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))] [HarmonyWrapSafe]
file static class FinishPieces
{
    [UsedImplicitly] [HarmonyPostfix]
    private static void Postfix(ZNetScene __instance)
    {
        var woodPlaceEffect = piece("woodwall").m_placeEffect;
        var woodwallWN = wearNTear("woodwall");
        TE_coalGenerator(woodPlaceEffect, woodwallWN);
        TE_woodStorage(woodPlaceEffect, woodwallWN);
        TE_woodWire(woodPlaceEffect, woodwallWN);
    }

    private static void TE_woodWire(EffectList woodPlaceEffect, WearNTear woodwallWN)
    {
        var wirePiece = piece("TE_woodWire");
        var wirePieceWN = wearNTear("TE_woodWire");
        wirePiece.m_placeEffect = woodPlaceEffect;
        wirePieceWN.m_hitEffect = woodwallWN.m_hitEffect;
        FixShaders(wirePiece);
        wirePiece.gameObject.GetOrAddComponent<MonoWire>();
    }

    private static void TE_woodStorage(EffectList woodPlaceEffect, WearNTear woodwallWN)
    {
        var storagePiece = piece("TE_woodenStorage");
        var storagePieceWN = wearNTear("TE_woodenStorage");
        storagePiece.m_placeEffect = woodPlaceEffect;
        storagePieceWN.m_hitEffect = woodwallWN.m_hitEffect;
        FixShaders(storagePiece);
        storagePiece.gameObject.GetOrAddComponent<MonoStorage>();
    }

    private static void TE_coalGenerator(EffectList woodPlaceEffect, WearNTear woodwallWN)
    {
        var generatorPiece = piece("TE_coalGenerator");
        var generatorPieceWN = wearNTear("TE_coalGenerator");
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