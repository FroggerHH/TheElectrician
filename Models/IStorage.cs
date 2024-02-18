namespace TheElectrician.Models;

public interface IStorage : IWireConnectable
{
    UnityEvent onStorageChanged { get; }
    UnityEvent<string, float> onItemAdded { get; }
    UnityEvent<string, float>  onItemRemoved { get; }
    Dictionary<string, float> GetStored();
    void SetStored(string key, float stored);
    bool Add(string key, float amount);
    bool Remove(string key, float amount);
    int GetPowerCapacity();
    int GetOtherCapacity();
    bool IsFull(bool power);
    bool IsEmpty(bool power);
    void Clear();
    bool CanAdd(string key, float amount);
    bool CanRemove(string key, float amount);
    float FreeSpace(bool power);
    string[] GetAllowedKeys();
    bool CanAccept(string key);

    float Count(string key);
    bool TransferTo(IStorage otherStorage, string key, float amount);
    bool GetFrom(IStorage otherStorage, string key, float amount);
    bool GetFrom(ZDO container, string key, int amount);
}