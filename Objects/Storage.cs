using TheElectrician.Models;
using TheElectrician.Models.Settings;
using UnityEngine.Events;

namespace TheElectrician.Objects;

public class Storage : WireConnectable, IStorage
{
    public UnityEvent onStorageChanged { get; private set; }
    private Dictionary<string, float> cashedStored = new();
    private StorageSettings storageSettings;

    public override void InitSettings(ElectricObjectSettings sett)
    {
        base.InitSettings(sett);
        storageSettings = GetSettings<StorageSettings>();
        if (storageSettings is null)
            DebugError($"Storage.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not StorageSettings");
    }

    public override void InitData()
    {
        base.InitData();
        GetStored();
        onStorageChanged = new UnityEvent();
    }


    public Dictionary<string, float> GetStored()
    {
        var savedString = GetZDO().GetString(Consts.storageKey, "-1");
        if (savedString == "-1")
        {
            cashedStored = new Dictionary<string, float>();
            return cashedStored;
        }

        cashedStored = savedString
            .Split(';')
            .Select(x => x.Split(':'))
            .ToDictionary(x => x[0], x => float.Parse(x[1]));
        return cashedStored;
    }

    public void SetStored(string key, float stored)
    {
        var clamp = Clamp(stored, 0, GetCapacity());
        cashedStored[key] = clamp;
        UpdateCurrentStored();
    }

    public bool Add(string key, float amount)
    {
        if (!CanAdd(amount)) return false;

        if (cashedStored.ContainsKey(key))
            cashedStored[key] += amount;
        else cashedStored.Add(key, amount);

        UpdateCurrentStored();
        return true;
    }

    public bool Remove(string key, float amount)
    {
        if (!CanRemove(key, amount))
        {
            DebugError($"Can't remove  {amount} {key} from storage");
            return false;
        }

        cashedStored[key] -= amount;

        UpdateCurrentStored();
        return true;
    }

    public int GetCapacity() => storageSettings.capacity;

    public bool IsFull()
    {
        var capacity = GetCapacity();
        return cashedStored.Sum(x => x.Value) >= capacity;
    }

    public bool IsEmpty() { return cashedStored.Sum(x => x.Value) == 0; }

    public void Clear()
    {
        cashedStored.Clear();
        UpdateCurrentStored();
    }

    public bool CanAdd(float amount)
    {
        if (amount < 0)
        {
            DebugError("Can't add negative amount");
            return false;
        }

        if (amount == 0)
        {
            DebugError("Can't add zero amount");
            return false;
        }

        return cashedStored.Sum(x => x.Value) + amount <= GetCapacity();
    }

    public bool CanRemove(string key, float amount)
    {
        if (amount < 0)
        {
            DebugError("Can't remove negative amount");
            return false;
        }

        if (amount == 0)
        {
            DebugError("Can't remove zero amount");
            return false;
        }

        if (cashedStored.TryGetValue(key, out var current))
            return current - amount >= 0;

        return false;
    }

    public float FreeSpace() { return GetCapacity() - cashedStored.Sum(x => x.Value); }

    public float Count(string key) { return cashedStored.TryGetValue(key, out var current) ? current : 0; }

    public bool TransferTo(IStorage otherStorage, string key, float amount)
    {
        if (otherStorage.CanAdd(amount) && Remove(key, amount))
        {
            otherStorage.Add(key, amount);
            return true;
        }

        return false;
    }

    public bool GetFrom(IStorage otherStorage, string key, float amount)
    {
        if (otherStorage.CanRemove(key, amount) && CanAdd(amount))
        {
            Add(key, amount);
            return true;
        }

        return false;
    }

    public bool GetFrom(ZDO container, string key, int amount) { throw new NotImplementedException(); }

    public override bool CanConnectOnlyToWires() => true;

    public override string ToString()
    {
        if (!IsValid()) return "Uninitialized Storage";
        return $"Storage {GetId()} stored: {cashedStored.GetString()}";
    }

    private void UpdateCurrentStored()
    {
        var join = string.Join(";", cashedStored.Select(x => $"{x.Key}:{x.Value}"));
        if (!IsValid()) return;
        onStorageChanged?.Invoke();
        GetZDO().Set(Consts.storageKey, join);
    }
}