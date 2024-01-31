using TheElectrician.Models;
using TheElectrician.Objects.Mono.Wire;

namespace TheElectrician.Objects.Mono;

public class MonoStorage : ElectricMono, Interactable
{
    public static List<MonoStorage> AllStorages = new();
    public IStorage storage { get; private set; }

    public override void OnDestroy()
    {
        if (!netView || !netView.IsValid() || !storage.IsValid()) return;
        DropAll();
        AllStorages.Remove(this);
    }

    public virtual bool Interact(Humanoid user, bool hold, bool alt)
    {
        return ConnectDisconnectWire(hold, alt, storage);
    }

    public virtual bool UseItem(Humanoid user, ItemData item) { return false; }

    public override void Load()
    {
        storage = Library.GetObject(netView.GetZDO()) as IStorage;
        AllStorages.Add(this);
    }

    public override string GetHoverText()
    {
        var sb = new StringBuilder();
        sb.AppendLine(piece.m_name.Localize());
        if (m_debugMode)
        {
            sb.AppendLine($"ID: {storage.GetId()}");
            var connected = storage.GetConnections().Select(x => x?.GetId().ToString() ?? "null").ToList();
            sb.AppendLine($"Connected: {(connected.Count > 0 ? connected.GetString() : "none")}");
        }

        sb.AppendLine();
        sb.AppendLine($"${ModName}_storage_capacity".Localize() + ": " + storage.GetCapacity());
        sb.AppendLine(StoredText(storage));
        return sb.ToString();
    }

    internal static string StoredText(IStorage storage, bool addEmptyMessage = false)
    {
        var sb = new StringBuilder();

        var currentStored = storage.GetStored();
        if (currentStored.Sum(x => x.Value) > 0) sb.AppendLine($"${ModName}_storage_stored".Localize());
        else if (addEmptyMessage) return $"${ModName}_storage_empty".Localize();
        else return string.Empty;
        foreach (var itemPair in currentStored)
        {
            var prefabName = itemPair.Key;
            var count = itemPair.Value;
            if (!prefabName.IsGood()) continue;
            string itemName;
            if (prefabName == Consts.storagePowerKey)
            {
                itemName = $"${ModName}_power";
            } else
            {
                var sharedData = ZNetScene.instance.GetPrefab(prefabName)
                    ?.GetComponent<ItemDrop>()?.m_itemData?.m_shared;
                if (sharedData is null) continue;
                itemName = sharedData.m_name;
            }

            sb.AppendLine($" - {itemName}: {count}");
        }

        return sb.ToString().Localize();
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