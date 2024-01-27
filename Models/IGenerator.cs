namespace TheElectrician.Models;

public interface IGenerator : IStorage
{
    float GetPowerPerTick();
    void SetPowerPerTick(float power);
    void SetActive(bool active);
    bool IsActive();

    //Requires fuel to work
    bool HasFuel();
    bool AddFuel(float amount);
    bool RemoveFuel(float amount);
    float GetFuelStored();
    string GetFuelItem();
    void SetFuelItem(string item);
    void SetFuel(float amount);
    int GetMaxFuel();
    void SetMaxFuel(int amount);
    float GetFuelPerTick();
    void SetFuelPerTick(float amount);
}