namespace TheElectrician.Objects.Mono;

public class MonoItemPipe : ElectricMono, Hoverable, Interactable
{
    private IItemPipe pipe { get; set; }

    public override string GetHoverText()
    {
        StringBuilder sb = new();
        sb.AppendLine(piece.m_name.Localize());
        return sb.ToString();
    }

    public bool Interact(Humanoid user, bool hold, bool alt) { return false; }


    public bool UseItem(Humanoid user, ItemData item) { return false; }

    public override Guid GetId()
    {
        if (pipe == null || !pipe.IsValid()) return Guid.Empty;
        return pipe.GetId();
    }

    public override void Load()
    {
        pipe = Library.GetObject(netView.GetZDO()) as IItemPipe;
        if (pipe is null)
        {
            DebugError($"Pipe {netView.GetZDO()} not found");
            foreach (var child in gameObject.GetComponentsInChildren<Renderer>()) child.material.color = Color.red;
            return;
        }
    }
} 