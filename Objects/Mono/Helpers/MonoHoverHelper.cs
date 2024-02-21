namespace TheElectrician.Objects.Mono.Helpers;

public static class MonoHoverHelper
{
    public static string CapacityText(IStorage storage)
    {
        return string.Format(
                $"${ModName}_storage_capacity_format".Localize(),
                storage.GetPowerCapacity(),
                storage.GetOtherCapacity())
            .Localize();
    }

    public static string StoredText(IStorage storage, bool addEmptyMessage = false)
    {
        var sb = new StringBuilder();

        var currentStored = storage.GetStored();
        if (currentStored.Sum(x => x.Value) > 0) sb.AppendLine($"${ModName}_storage_stored".Localize());
        else if (addEmptyMessage) return $"${ModName}_storage_empty".Localize();
        else return string.Empty;
        foreach (var itemPair in currentStored)
        {
            var prefabName = itemPair.Key;
            var count = Math.Round(itemPair.Value, TheConfig.RoundingPrecision);
            //Show 0 if less than Consts.minPower
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

    public static bool DebugText(IStorage storage, out string result, bool id = true, bool connections = false)
    {
        result = string.Empty;
        if (!m_debugMode) return false;
        if (storage is null) return false;
        var sb = new StringBuilder();

        if (id) sb.AppendLine($"ID: {storage.GetId()}");
        if (connections)
        {
            var connected = storage.GetConnections().Select(x => x?.GetId().ToString() ?? "null").ToList();
            sb.AppendLine($"Connected: {(connected.Count > 0 ? connected.GetString() : "no one")}");
        }

        result = sb.ToString();
        return true;
    }
}