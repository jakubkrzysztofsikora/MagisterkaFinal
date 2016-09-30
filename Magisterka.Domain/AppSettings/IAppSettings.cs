namespace Magisterka.Domain.AppSettings
{
    public interface IAppSettings
    {
        string RaportPath { get; }
        int RandomGraphDefaultNodeNumber { get; }
        int RandomGraphDefaultMaxNeighborsForNode { get; }
        int RandomGraphMinNeighborNumber { get; }
        int RandomGraphMinEdgeCost { get; }
        int RandomGraphMaxEdgeCost { get; }
}
}
