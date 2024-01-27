using TheElectrician.Models;

namespace TheElectrician.Objects.Mono;

public class MonoStorage : MonoBehaviour, Hoverable
{
    public IStorage storage { get; private set; }
    public ZNetView netView { get; private set; }


    private void Awake()
    {
        netView = GetComponent<ZNetView>();
        storage = Library.GetObject(netView.GetZDO()) as IStorage;
    }

    public string GetHoverText()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"${ModName}_storage".Localize());
        sb.AppendLine();
        sb.AppendLine($"${ModName}_storage_capacity".Localize() + ": " + storage.GetCapacity());
        sb.AppendLine(StoredText(storage));
        return sb.ToString();
    }

    internal static string StoredText(IStorage storage)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"${ModName}_storage_stored".Localize());
        var currentStored = storage.CurrentStored();
        foreach (var itemPair in currentStored)
        {
            var prefabName = itemPair.Key;
            var count = itemPair.Value;
            if (!prefabName.IsGood()) continue;
            string itemName;
            if (prefabName == Consts.storagePowerKey) itemName = $"${ModName}_power".Localize();
            else
            {
                SharedData sharedData = ZNetScene.instance.GetPrefab(prefabName)
                    ?.GetComponent<ItemDrop>()?.m_itemData?.m_shared;
                if (sharedData is null) continue;
                itemName = sharedData.m_name.Localize();
            }

            sb.AppendLine(itemName + ": " + count);
        }

        return sb.ToString();
    }

    public string GetHoverName() { return $"${ModName}_storage".Localize(); }

    public bool UseItem(Humanoid user, ItemData item) { throw new NotImplementedException(); }

    public void OnDestroyed()
    {
        if (!netView.IsOwner()) return;
        DropAll();
    }

    private void DropAll()
    {
        storage.Remove(Consts.storagePowerKey, storage.Count(Consts.storagePowerKey));
        var transform1 = transform;
        foreach (var item in storage.CurrentStored())
        {
            var prefabName = item.Key;
            var count = item.Value;
            if (!prefabName.IsGood()) continue;
            var prefab = ZNetScene.instance.GetPrefab(prefabName);
            if (prefab == null) continue;
            var itemDrop = Instantiate(prefab, transform1.position, transform1.rotation).GetComponent<ItemDrop>();
            itemDrop.m_itemData.m_stack = FloorToInt(count);
            ItemDrop.OnCreateNew(itemDrop);
        }
    }
}