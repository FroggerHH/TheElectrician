using TheElectrician.Helpers;
using TheElectrician.Objects.Mono;
using static TheElectrician.Helpers.ShaderHelper;

namespace TheElectrician.Patch;

[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))] [HarmonyWrapSafe]
file static class FinishPieces
{
    private static EffectList woodPlaceEffect;
    private static WearNTear woodwallWN;
    private static WearNTear smelterWN;
    private static Smelter smelter;

    [UsedImplicitly] [HarmonyPostfix]
    private static void Postfix(ZNetScene __instance)
    {
        ShaderHelper.Init();

        woodPlaceEffect = piece("woodwall").m_placeEffect;
        woodwallWN = wearNTear("woodwall");
        smelter = prefab("smelter").GetComponent<Smelter>();
        smelterWN = smelter.GetComponent<WearNTear>();
        TE_coalGenerator();
        TE_woodStorage();
        TE_woodWire();
        TE_stoneFurnace();
        TE_tinPipe();
    }

    private static void TE_tinPipe()
    {
        var _piece = piece("TE_tinPipe");
        var _wearNTear = wearNTear("TE_tinPipe");
        _piece.m_placeEffect = woodPlaceEffect;
        _wearNTear.m_hitEffect = woodwallWN.m_hitEffect;
        FixShaders(_piece.gameObject);
        _piece.gameObject.GetOrAddComponent<MonoItemPipe>();
    }

    private static void TE_woodWire()
    {
        var wirePiece = piece("TE_woodWire");
        var wirePieceWN = wearNTear("TE_woodWire");
        wirePiece.m_placeEffect = woodPlaceEffect;
        wirePieceWN.m_hitEffect = woodwallWN.m_hitEffect;
        FixShaders(wirePiece.gameObject);
        wirePiece.gameObject.GetOrAddComponent<MonoWire>();
    }

    private static void TE_woodStorage()
    {
        var storagePiece = piece("TE_woodenStorage");
        var storagePieceWN = wearNTear("TE_woodenStorage");
        storagePiece.m_placeEffect = woodPlaceEffect;
        storagePieceWN.m_hitEffect = woodwallWN.m_hitEffect;
        var go = storagePiece.gameObject;
        FixShaders(go);
        go.GetOrAddComponent<MonoStorage>();
    }

    private static void TE_coalGenerator()
    {
        var generatorPiece = piece("TE_coalGenerator");
        var generatorPieceWN = wearNTear("TE_coalGenerator");
        generatorPiece.m_placeEffect = piece("smelter").m_placeEffect;
        generatorPieceWN.m_hitEffect = smelterWN.m_hitEffect;
        var go = generatorPiece.gameObject;
        FixShaders(go);
        FixSmoke(go);
        var monoGenerator = go.GetOrAddComponent<MonoGenerator>();
        monoGenerator.addEffect = smelter.m_oreAddedEffects;
    }

    private static void TE_stoneFurnace()
    {
        var stoneFurnacePiece = piece("TE_stoneFurnace");
        var stoneFurnaceWN = wearNTear("TE_stoneFurnace");
        stoneFurnacePiece.m_placeEffect = piece("smelter").m_placeEffect;
        stoneFurnaceWN.m_hitEffect = smelterWN.m_hitEffect;
        FixShaders(stoneFurnacePiece);
        var go = stoneFurnacePiece.gameObject;
        FixSmoke(go);
        FixGuidePoint(go);
        var monoFurnace = go.GetOrAddComponent<MonoFurnace>();
        monoFurnace.doneEffect = smelter.m_produceEffects;
        monoFurnace.addEffect = smelter.m_oreAddedEffects;

        DebugWarning($"monoFurnace.doneEffect {monoFurnace.doneEffect?.ToString() ?? "null"}");
        DebugWarning($"MonoFurnace.all {ElectricMono.GetAll().OfType<IFurnace>().GetString()}");
    }
}