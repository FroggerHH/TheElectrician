using System.Globalization;
using TheElectrician.Models;
using TheElectrician.Models.Settings;

namespace TheElectrician.Objects;

public class Storage : IStorage
{
    private readonly ZDO zdo;
    private Dictionary<string, float> cashedStored;
    private HashSet<IWire> cashedConnectedWires = new();

    public Storage(ZDO ZDO)
    {
        zdo = ZDO;
        Library.TryGiveId(this);
        CurrentStored();
        GetConnectedWires();
    }

    public virtual void Update() { }

    public virtual void InitSettings(ElectricObjectSettings settings)
    {
        if (settings is not StorageSettings storageSettings)
        {
            DebugError("InitSettings: Storage settings is not StorageSettings");
            return;
        }

        SetCapacity(storageSettings.capacity);
    }

    public Guid GetId() { return Guid.Parse(zdo.GetString(Consts.electricObjectIdKey, Guid.Empty.ToString())); }

    public Dictionary<string, float> CurrentStored()
    {
        var savedString = GetZDO().GetString(Consts.storageKey, "-1");
        if (savedString == "-1")
        {
            cashedStored = new();
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
        if (!CanAdd(amount))
        {
            DebugError($"Can't add {amount} {key} to storage");
            return false;
        }

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

    public int GetCapacity()
    {
        var capacity = GetZDO().GetInt(Consts.capacityKey, -1);
        if (capacity == -1)
        {
            GetZDO().Set(Consts.capacityKey, 1);
            DebugWarning($"Capacity of the storage {GetZDO()} has not defined, set to default: 1");
            return 1;
        }

        return capacity;
    }

    public void SetCapacity(int capacity) { GetZDO().Set(Consts.capacityKey, capacity); }

    public void AddCapacity(int capacity) { SetCapacity(GetCapacity() + capacity); }

    public void RemoveCapacity(int capacity)
    {
        var resultCapacity = GetCapacity() - capacity;
        if (resultCapacity < 0)
        {
            DebugError($"Capacity of the storage {GetZDO()} can't be less than zero");
            return;
        }

        SetCapacity(resultCapacity);
    }

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

    public float FreeSpace() => GetCapacity() - cashedStored.Sum(x => x.Value);

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

    public HashSet<IWire> GetConnectedWires()
    {
        cashedConnectedWires = Library.GetAllObjects<IWire>().Where(x => x.GetConnections().Contains(this)).ToHashSet();
        return cashedConnectedWires;
    }

    public void AddConnectionWire(IWire wire)
    {
        if (cashedConnectedWires.Contains(wire)) return;
        cashedConnectedWires.Add(wire);
        wire.AddConnection(this);
        UpdateConnections();
    }

    private void UpdateConnections() => cashedConnectedWires = cashedConnectedWires.Where(x => x is not null).ToHashSet();

    public void RemoveConnectionWire(IWire wire)
    {
        if (!wire.GetConnections().Contains(this)) return;
        cashedConnectedWires.Remove(wire);
        wire.RemoveConnection(this);
        UpdateConnections();
    }

    public ZDO GetZDO() { return zdo; }

    private void UpdateCurrentStored()
    {
        var join = string.Join(";", cashedStored.Select(x => $"{x.Key}:{x.Value}"));
        GetZDO().Set(Consts.storageKey, join);
    }

    public override string ToString() { return $"Storage {GetId()} stored: {cashedStored.GetString()}"; }
}