using TheElectrician.Objects.Mono;

namespace TheElectrician.Extensions;

[PublicAPI]
public static class PlayerExtension
{
    [CanBeNull]
    public static IElectricObject GetHoveringEO(this Player player)
    {
        var hoverObject = player.GetHoverObject()?.GetComponentInParent<ZNetView>()?.gameObject;
        if (!hoverObject) return null;

        var mono = ElectricMono.GetAll().Find(x => x.gameObject == hoverObject);
        if (mono is null) return null;
        return Library.GetObject(mono.GetId());
    }
}