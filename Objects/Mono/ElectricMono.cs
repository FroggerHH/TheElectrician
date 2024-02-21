using System.Text.RegularExpressions;
using TheElectrician.Patch;

namespace TheElectrician.Objects.Mono;

public abstract class ElectricMono : MonoBehaviour, Hoverable
{
    private static readonly List<ElectricMono> all = [];
    protected ZNetView netView { get; private set; }
    protected Piece piece { get; private set; }

    public virtual void OnDestroy() { all.Remove(this); }

    public abstract string GetHoverText();

    public static void UpdateLevelText(ILevelable eo, ElectricMono mono)
    {
        if (mono?.piece == null || eo is null) return;

        mono.piece.m_name = Regex.Replace(mono.piece.m_name, ShowPieceLevel.pattern, "");
        var format = string.Format(ShowPieceLevel.addPattern, ShowPieceLevel.color, eo.GetLevel());
        mono.piece.m_name += format;
    }

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