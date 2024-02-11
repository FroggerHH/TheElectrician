using TheElectrician.Objects.Consumers.Furnace;

namespace TheElectrician.Models;

public interface IFurnace : IStorage, IConsumer
{
    UnityEvent onProgressStarted { get; }
    UnityEvent onProgressChanged { get; }
    UnityEvent onProgressCompleted { get; }
    FurnaceState GetState();
    bool IsInWorkingState();
    (int start, int end) GetProgress();
    FurnaceRecipe GetCurrentRecipe();
    bool HaveEnoughPower();
    bool HaveEnoughPower(FurnaceRecipe recipe);
}