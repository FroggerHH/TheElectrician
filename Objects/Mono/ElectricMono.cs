namespace TheElectrician.Objects.Mono;

public abstract class ElectricMono : MonoBehaviour, Hoverable
{
    private static readonly List<ElectricMono> all = [];
    protected ZNetView netView { get; private set; }
    protected Piece piece { get; private set; }

    public virtual void OnDestroy() { all.Remove(this); }
    public abstract string GetHoverText();

    public virtual string GetHoverName() { return piece.m_name.Localize(); }
    public static List<ElectricMono> GetAll() { return all; }

    public abstract Guid GetId();

    public virtual void SetUp()
    {
        netView = GetComponent<ZNetView>();
        piece = GetComponent<Piece>();
        if (!netView.IsValid()) return;
        all.Add(this);
    }

    public abstract void Load();
}