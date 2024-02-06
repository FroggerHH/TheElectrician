namespace TheElectrician.Helpers;

[PublicAPI]
public static class ShaderHelper
{
    private static GameObject smokePrefab_Original;
    private static GameObject raven_Original;

    internal static void Init()
    {
        smokePrefab_Original = prefab("charcoal_kiln").GetComponentInChildren<SmokeSpawner>(true).m_smokePrefab;
        raven_Original = prefab("piece_workbench").GetComponentInChildren<GuidePoint>(true).m_ravenPrefab;
    }

    public static void FixSmoke(GameObject obj)
    {
        var smoke = obj.GetComponentInChildren<SmokeSpawner>(true);
        smoke.m_smokePrefab = smokePrefab_Original;
    }

    public static void FixGuidePoint(GameObject obj)
    {
        var guidePoint = obj.GetComponentInChildren<GuidePoint>(true);
        guidePoint.m_ravenPrefab = raven_Original;
    }

    public static void FixShaders(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>(true);
        foreach (var ren in renderers) ren.material.shader = Shader.Find(ren.material.shader.name);
    }

    public static void FixShaders(Component obj) => FixShaders(obj.gameObject);
}