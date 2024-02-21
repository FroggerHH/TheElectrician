using TheElectrician.Objects.Mono.Helpers;
using TheElectrician.Objects.Mono.Wire;

namespace TheElectrician.Objects.Mono;

public class MonoStorage : ElectricMono, Interactable
{
    private IStorage storage { get; set; }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (!netView || !netView.IsValid() || !storage.IsValid()) return;
        DropAll();
    }

    public virtual bool Interact(Humanoid user, bool hold, bool alt)
    {
        return ConnectDisconnectWire(hold, alt, storage);
    }

    public virtual bool UseItem(Humanoid user, ItemData item) { return false; }

    public override Guid GetId()
    {
        if (storage == null || !storage.IsValid()) return Guid.Empty;
        return storage.GetId();
    }

    public override void Load() { storage = Library.GetObject(netView.GetZDO()) as IStorage; }

    public override string GetHoverText()
    {
        var sb = new StringBuilder();
        sb.AppendLine(piece.m_name.Localize());
        if (MonoHoverHelper.DebugText(storage, out var debugText)) sb.AppendLine(debugText);

        // if (storage.IsFull(true) || storage.IsEmpty(false))
        //     sb.AppendLine($"<color=#F448B2>${ModName}_storage_is_full </color>".Localize());

        sb.AppendLine();
        sb.AppendLine(MonoHoverHelper.CapacityText(storage));
        sb.AppendLine(MonoHoverHelper.StoredText(storage));
        return sb.ToString();
    }

    public static bool ConnectDisconnectWire(bool hold, bool alt, IStorage connectingToStorage)
    {
        if (hold) return false;
        if (alt) return false;

        var allWires = Library.GetAllObjects<IWire>();
        var wires = allWires.FindAll(x => x.GetState() == WireState.Connecting);
        if (wires.Count > 1)
        {
            DebugError($"There are {wires.Count} connecting to managed object wires. This should not happen");
            wires.ForEach(x => x.SetState(WireState.Idle));
            return false;
        }

        IWire wire;
        if (wires.Count == 0)
        {
            wires = allWires.FindAll(x => x.GetState() == WireState.Disconnecting);
        } else
        {
            //Connecting
            wire = wires.First();
            wire.AddConnection(connectingToStorage);
            m_localPlayer?.Message(MessageHud.MessageType.TopLeft, "<color=#95E455>Connected</color>");
            return true;
        }

        //Disconnecting
        if (wires.Count > 1)
        {
            DebugError($"There are {wires.Count} disconnecting to managed object wires. This should not happen");
            wires.ForEach(x => x.SetState(WireState.Idle));
            return false;
        }

        if (wires.Count == 0) return false;

        wire = wires.First();
        wire.RemoveConnection(connectingToStorage);
        m_localPlayer?.Message(MessageHud.MessageType.TopLeft, "<color=#95E455>Disconnected</color>");
        return true;
    }

    private void DropAll()
    {
        storage.Remove(Consts.storagePowerKey, storage.Count(Consts.storagePowerKey));
        var transform1 = transform;
        foreach (var item in storage.GetStored())
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