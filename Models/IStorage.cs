namespace TheElectrician.Models;

public interface IStorage : IElectricObject
{
    Dictionary<string, float> CurrentStored();
    void SetStored(string key, float stored);
    bool Add(string key, float amount);
    bool Remove(string key, float amount);
    int GetCapacity();
    void SetCapacity(int capacity);
    void AddCapacity(int capacity);
    void RemoveCapacity(int capacity);
    bool IsFull();
    bool IsEmpty();
    void Clear();
    bool CanAdd(float amount);
    bool CanRemove(string key, float amount);

    float Count(string key);
    bool TransferTo(IStorage otherStorage, string key, float amount);
    bool GetFrom(IStorage otherStorage, string key, float amount);
    bool GetFrom(ZDO container, string key, int amount);
}