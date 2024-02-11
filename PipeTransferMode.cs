namespace TheElectrician;

[Flags]
public enum PipeTransferMode
{
    Items = 1,
    Power = 2,
    Liquid = 4,
    All = Items | Power | Liquid
}