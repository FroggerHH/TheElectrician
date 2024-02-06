namespace TheElectrician.Helpers;

[PublicAPI]
public static class PrefabHelper
{
    public static GameObject prefab(string name) { return ZNetScene.instance?.GetPrefab(name); }
    public static ItemDrop item(string name) { return prefab(name)?.GetComponent<ItemDrop>(); }

    public static Piece piece(string name) { return prefab(name)?.GetComponent<Piece>(); }

    public static WearNTear wearNTear(string name) { return prefab(name)?.GetComponent<WearNTear>(); }
}