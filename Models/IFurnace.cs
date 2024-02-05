using TheElectrician.Objects.Consumers.Furnace;
using UnityEngine.Events;

namespace TheElectrician.Models;

public interface IFurnace : IStorage, IConsumer
{
    UnityEvent onProgressAdded { get; }
    UnityEvent onProgressCompleted { get; }
    FurnaceState GetState();
    bool IsInWorkingState();
    RangeInt GetProgress();
    FurnaceRecipe GetCurrentRecipe();
    bool HaveEnoughPower();
    bool HaveEnoughPower(FurnaceRecipe recipe);
}