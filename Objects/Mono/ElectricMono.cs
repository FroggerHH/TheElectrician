namespace TheElectrician.Objects.Mono;

public abstract class ElectricMono : MonoBehaviour, Hoverable
{
    public ZNetView netView { get; private set; }
    public Piece piece { get; private set; }
    public virtual void OnDestroy() { }
    public abstract string GetHoverText();

    public virtual string GetHoverName() { return piece.m_name.Localize(); }

    public virtual void SetUp()
    {
        netView = GetComponent<ZNetView>();
        piece = GetComponent<Piece>();
        if (!netView.IsValid()) return;
    }

    public abstract void Load();
}