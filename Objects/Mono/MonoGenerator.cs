namespace TheElectrician.Objects.Mono;

public class MonoGenerator : ElectricMono, Hoverable, Interactable
{
    private static readonly float HoldRepeatInterval = 0.2f;
    private GameObject enabledVisual;
    private GameObject itemPreview;
    private float m_lastUseTime;
    private IGenerator generator;

    internal EffectList addEffect;

    public override string GetHoverText()
    {
        if (generator == null) return string.Empty;
        var sb = new StringBuilder();
        var fuelItemPrefabName = generator.GetFuelItem();
        var fuelItemName = string.Empty;
        var fuelItemPrefab = ZNetScene.instance.GetPrefab(fuelItemPrefabName)?.GetComponent<ItemDrop>();
        if (fuelItemPrefab != null) fuelItemName = fuelItemPrefab.m_itemData.m_shared.m_name;

        sb.AppendLine(piece.m_name.Localize());
        if (m_debugMode)
        {
            sb.AppendLine($"ID: {generator.GetId()}");
            var connected = generator.GetConnections().Select(x => x?.GetId().ToString() ?? "null").ToList();
            sb.AppendLine($"Connected: {(connected.Count > 0 ? connected.GetString() : "no one")}");
        }

        sb.AppendLine();
        sb.AppendLine($"[<color=yellow><b>$KEY_Use</b></color>] $piece_smelter_add {fuelItemName}".Localize());
        sb.AppendLine($"${ModName}_storage_capacity".Localize() + ": " + generator.GetCapacity());

        if (!generator.CanAdd(Consts.storagePowerKey, generator.GetPowerPerTick()))
            sb.AppendLine($"<color=#F448B2>${ModName}_generator_is_full  </color>".Localize());

        //Fuel item
        if (fuelItemName.IsGood())
            sb.AppendLine(string.Format($"${ModName}_generator_uses_fuel".Localize(), fuelItemName.Localize()));
        sb.AppendLine(string.Format($"${ModName}_generator_power_per_tick".Localize(), generator.GetPowerPerTick()));
        sb.AppendLine(string.Format($"${ModName}_generator_fuel_per_tick".Localize(), generator.GetFuelPerTick()));

        //Stored items
        sb.AppendLine(MonoStorage.StoredText(generator));

        return sb.ToString();
    }

    public bool Interact(Humanoid user, bool hold, bool alt)
    {
        if (MonoStorage.ConnectDisconnectWire(hold, alt, generator)) return true;
        if (hold && (HoldRepeatInterval <= 0.0 || Time.time - m_lastUseTime < HoldRepeatInterval))
            return false;
        m_lastUseTime = Time.time;

        var fuelItemPrefabName = generator.GetFuelItem();
        var fuelItem = ZNetScene.instance.GetPrefab(fuelItemPrefabName)?.GetComponent<ItemDrop>()?.m_itemData
            .m_shared.m_name;
        if (!user.GetInventory().HaveItem(fuelItem))
        {
            user.Message(MessageHud.MessageType.Center, "$msg_donthaveany " + fuelItem);
            return false;
        }

        var addResult = generator.AddFuel(1);
        if (addResult)
        {
            user.GetInventory().RemoveItem(fuelItem, 1);
            addEffect?.Create(transform.position, Quaternion.identity);
        }

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

        var addResult = generator.AddFuel(1);
        if (addResult)
        {
            user.GetInventory().RemoveItem(fuelItem, 1);
            addEffect?.Create(transform.position, Quaternion.identity);
        }

        return addResult;
    }

    public override Guid GetId()
    {
        if (generator == null || !generator.IsValid()) return Guid.Empty;
        return generator.GetId();
    }

    public override void SetUp()
    {
        base.SetUp();
        enabledVisual = transform.Find("EnabledVisual")?.gameObject;
        if (!enabledVisual)
        {
            enabledVisual = new GameObject("EnabledVisual");
            enabledVisual.transform.parent = transform;
            enabledVisual.transform.localPosition = Vector3.zero;

            DebugWarning($"Generator {gameObject.GetPrefabName()} has no enabled visual");
        }

        enabledVisual.SetActive(false);

        itemPreview = transform.Find("ItemPreview")?.gameObject;
        if (!itemPreview)
        {
            itemPreview = new GameObject("ItemPreview");
            itemPreview.transform.parent = transform;
            itemPreview.transform.localPosition = Vector3.zero;

            DebugWarning($"Generator {gameObject.GetPrefabName()} has no item preview");
        }
    }

    public override void Load()
    {
        if (!netView.IsValid()) return;
        generator = Library.GetObject(netView.GetZDO()) as IGenerator;
        if (generator is null) return;

        if (generator.GetFuelItem().IsGood())
        {
            var item = ZNetScene.instance.GetPrefab(generator.GetFuelItem())?.transform.Find("attach");
            if (!item) return;

            Instantiate(item.gameObject, itemPreview.transform);
        }

        InvokeRepeating(nameof(UpdateVisual), 1.0f, 1.0f);
    }

    private void UpdateVisual()
    {
        enabledVisual.SetActive(generator.HasFuel());
        itemPreview.SetActive(generator.Count(generator.GetFuelItem()) > 0);
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
        foreach (var item in generator.GetStored())
        {
            var prefabName = item.Key;
            var count = item.Value;
            if (!prefabName.IsGood()) continue;
            var prefab = ZNetScene.instance.GetPrefab(prefabName);
            if (prefab == null) continue;
            var itemDrop = Instantiate(prefab, transform1.position, transform1.rotation).GetComponent<ItemDrop>();
            itemDrop.m_itemData.m_stack = FloorToInt(count);
            OnCreateNew(itemDrop);
        }
    }
}