namespace TheElectrician.Models;

public interface IGenerator : IStorage
{
    float GetPowerPerTick();
    void SetActive(bool active);
    bool IsActive();
    bool HasFuel();
    bool AddFuel(float amount);
    bool RemoveFuel(float amount);
    float GetFuelStored();
    string GetFuelItem();
    void SetFuel(float amount);
    int GetMaxFuel();
    float GetFuelPerTick();
}