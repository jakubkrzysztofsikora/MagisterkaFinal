namespace Magisterka.Domain.Graph.Pathfinding
{
    public interface IPathfinderFactory
    {
        Pathfinder CreatePathfinderWithAlgorithm(ePathfindingAlgorithms algorithm);
    }
}
