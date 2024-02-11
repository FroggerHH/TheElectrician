namespace TheElectrician.Models;

public interface ILevelable : IElectricObject
{
    UnityEvent onLevelChanged { get; }

    int GetStartLevel();
    int GetLevel();
    bool SetLevel(int level);
    bool AddLevel(int amount = 1);
    bool RemoveLevel(int amount = 1);
    int GetMaxLevel();
}