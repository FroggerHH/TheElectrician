using TheElectrician.Settings;
using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Objects;

public class Storage : WireConnectable, IStorage
{
    public UnityEvent onStorageChanged { get; private set; }
    public UnityEvent<string, float> onItemAdded { get; private set; }
    public UnityEvent<string, float> onItemRemoved { get; private set; }
    private Dictionary<string, float> cashedStored = new();
    private IStorageSettings storageSettings;

    public override void InitSettings(IElectricObjectSettings sett)
    {
        base.InitSettings(sett);
        storageSettings = GetSettings<IStorageSettings>();
        if (storageSettings is null)
            DebugError($"{GetType().Name}.InitSettings: Settings '{GetSettings()?.GetType().Name}' is not IStorageSettings");
    }

    public override void InitData()
    {
        base.InitData();
        GetStored();
        onStorageChanged = new();
        onItemAdded = new();
        onItemRemoved = new();
    }


    public Dictionary<string, float> GetStored()
    {
        var savedString = GetZDO().GetString(Consts.storageKey, "-1");
        if (savedString == "-1")
        {
            cashedStored = new Dictionary<string, float>();
            return cashedStored;
        }

        if (!savedString.IsGood()) return [];

        cashedStored = savedString
            .Split(';')
            .Select(x => x.Split(':'))
            .ToDictionary(x => x[0], x => float.Parse(x[1]));
        return cashedStored;
    }

    public void SetStored(string key, float stored)
    {
        var clamp = Clamp(stored, 0, key == Consts.storagePowerKey ? GetPowerCapacity() : GetOtherCapacity());
        cashedStored[key] = clamp;
        UpdateCurrentStored();
    }

    public bool Add(string key, float amount)
    {
        if (!CanAdd(key, amount)) return false;

        if (cashedStored.ContainsKey(key))
            cashedStored[key] += amount;
        else cashedStored.Add(key, amount);

        onItemAdded?.Invoke(key, amount);
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
        if (cashedStored[key] <= 0) cashedStored.Remove(key);

        onItemRemoved?.Invoke(key, amount);
        UpdateCurrentStored();
        return true;
    }

    public int GetPowerCapacity() => storageSettings.powerCapacity;
    public int GetOtherCapacity() => storageSettings.otherCapacity;

    public bool IsFull(bool power)
    {
        var capacity = power ? GetPowerCapacity() : GetOtherCapacity();
        var list = power
            ? cashedStored.Where(x => x.Key == Consts.storagePowerKey)
            : cashedStored.Where(x => x.Key != Consts.storagePowerKey);
        return capacity - list.Sum(x => x.Value) < 0;
    }

    public bool IsEmpty(bool power)
    {
        var list = power
            ? cashedStored.Where(x => x.Key == Consts.storagePowerKey)
            : cashedStored.Where(x => x.Key != Consts.storagePowerKey);

        return list.Sum(x => x.Value) <= float.Epsilon;
    }

    public void Clear()
    {
        cashedStored.Clear();
        UpdateCurrentStored();
    }

    public virtual bool CanAdd(string key, float amount)
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

        if (!CanAccept(key)) return false;

        var list = key == Consts.storagePowerKey
            ? cashedStored.Where(x => x.Key == Consts.storagePowerKey)
            : cashedStored.Where(x => x.Key != Consts.storagePowerKey);

        var capacity = key == Consts.storagePowerKey ? GetPowerCapacity() : GetOtherCapacity();

        return list.Sum(x => x.Value) + amount <= capacity;
    }

    public virtual bool CanRemove(string key, float amount)
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

    public float FreeSpace(bool power)
    {
        var capacity = power ? GetPowerCapacity() : GetOtherCapacity();
        var list = power
            ? cashedStored.Where(x => x.Key == Consts.storagePowerKey)
            : cashedStored.Where(x => x.Key != Consts.storagePowerKey);

        return capacity - list.Sum(x => x.Value);
    }

    public virtual string[] GetAllowedKeys() => storageSettings.allowedKeys;

    public bool CanAccept(string key) => GetAllowedKeys().Contains(key);

    public float Count(string key) { return cashedStored.TryGetValue(key, out var current) ? current : 0; }

    public bool TransferTo(IStorage otherStorage, string key, float amount)
    {
        if (otherStorage.CanAdd(key, amount) && Remove(key, amount))
        {
            otherStorage.Add(key, amount);
            return true;
        }

        return false;
    }

    public bool GetFrom(IStorage otherStorage, string key, float amount)
    {
        if (otherStorage.CanRemove(key, amount) && CanAdd(key, amount))
        {
            Add(key, amount);
            return true;
        }

        return false;
    }

    public virtual bool GetFrom(ZDO container, string key, int amount) { throw new NotImplementedException(); }

    public override bool CanConnectOnlyToWires() => true;

    private void UpdateCurrentStored()
    {
        cashedStored = cashedStored.Where(x => x.Value > 0).ToDictionary(x => x.Key, x => x.Value);
        if (!IsValid()) return;
        var join = string.Join(";", cashedStored.Select(x => $"{x.Key}:{x.Value}"));
        onStorageChanged?.Invoke();
        GetZDO().Set(Consts.storageKey, join);
    }
}