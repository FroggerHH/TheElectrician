using TheElectrician.Models;

namespace TheElectrician.Objects.Mono;

public class MonoGenerator : MonoBehaviour, Hoverable, Interactable
{
    private static readonly float HoldRepeatInterval = 0.2f;

    public IGenerator generator { get; private set; }
    public ZNetView netView { get; private set; }
    public Piece piece { get; private set; }
    private float m_lastUseTime;


    void Awake()
    {
        netView = GetComponent<ZNetView>();
        if (!netView.IsValid()) return;
        piece = GetComponent<Piece>();
        generator = Library.GetObject(netView.GetZDO()) as IGenerator;
    }

    public string GetHoverText()
    {
        if (generator == null) return string.Empty;
        var sb = new StringBuilder();
        var fuelItemPrefabName = generator.GetFuelItem();
        var fuelItemName = string.Empty;
        var fuelItemPrefab = ZNetScene.instance.GetPrefab(fuelItemPrefabName)?.GetComponent<ItemDrop>();
        if (fuelItemPrefab != null) fuelItemName = fuelItemPrefab.m_itemData.m_shared.m_name;

        sb.AppendLine(piece.m_name.Localize());
        sb.AppendLine();
        sb.AppendLine($"[<color=yellow><b>$KEY_Use</b></color>] $piece_smelter_add {fuelItemName}".Localize());
        sb.AppendLine($"${ModName}_storage_capacity".Localize() + ": " + generator.GetCapacity());

        //Fuel item
        if (fuelItemName.IsGood())
            sb.AppendLine(string.Format($"${ModName}_generator_uses_fuel".Localize(), fuelItemName.Localize()));
        sb.AppendLine(string.Format($"${ModName}_generator_power_per_tick".Localize(), generator.GetPowerPerTick()));
        sb.AppendLine(string.Format($"${ModName}_generator_fuel_per_tick".Localize(), generator.GetFuelPerTick()));

        //Stored items
        sb.AppendLine(MonoStorage.StoredText(generator));

        return sb.ToString();
    }

    public string GetHoverName() { return $"${ModName}_storage".Localize(); }

    public bool Interact(Humanoid user, bool hold, bool alt)
    {
        if (MonoStorage.ConnectDisconnectWire(hold, alt, generator)) return true;
        if (hold && (HoldRepeatInterval <= 0.0 || Time.time - m_lastUseTime < HoldRepeatInterval))
            return false;
        this.m_lastUseTime = Time.time;

        var fuelItemPrefabName = generator.GetFuelItem();
        var fuelItem = ZNetScene.instance.GetPrefab(fuelItemPrefabName)?.GetComponent<ItemDrop>()?.m_itemData
            .m_shared.m_name;
        if (!user.GetInventory().HaveItem(fuelItem))
        {
            user.Message(MessageHud.MessageType.Center, "$msg_donthaveany " + fuelItem);
            return false;
        }

        var addResult = generator.Add(fuelItemPrefabName, 1);
        if (addResult) user.GetInventory().RemoveItem(fuelItem, 1);
        return addResult;
    }

    public bool UseItem(Humanoid user, ItemData item)
    {
        var fuelItemPrefabName = generator.GetFuelItem();
        var fuelItem = ZNetScene.instance.GetPrefab(fuelItemPrefabName)?.GetComponent<ItemDrop>()?.m_itemData
            .m_shared.m_name;
        if (item.m_shared.m_name != fuelItem)
        {
            user.Message(MessageHud.MessageType.Center, "$msg_wrongitem");
            return false;
        }

        var addResult = generator.Add(fuelItemPrefabName, 1);
        if (addResult) user.GetInventory().RemoveItem(fuelItem, 1);
        return addResult;
    }

    public void OnDestroyed()
    {
        if (!netView.IsOwner()) return;
        DropAll();
    }

    private void DropAll()
    {
        generator.Remove(Consts.storagePowerKey, generator.Count(Consts.storagePowerKey));
        var transform1 = transform;
        foreach (var item in generator.CurrentStored())
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