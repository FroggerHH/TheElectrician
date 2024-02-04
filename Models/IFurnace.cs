using TheElectrician.Objects.Consumers.Furnace;
using UnityEngine.Events;

namespace TheElectrician.Models;

public interface IFurnace : IWireConnectable
{
    UnityEvent onProgressAdded { get; }
    UnityEvent onProgressCompleted { get; }
    FurnaceState GetState();
    bool IsInWorkingState();
}