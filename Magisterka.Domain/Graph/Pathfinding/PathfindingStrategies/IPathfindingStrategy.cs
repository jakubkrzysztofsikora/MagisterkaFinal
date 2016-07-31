using System.Collections.Generic;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies
{
    public interface IPathfindingStrategy
    {
        IEnumerable<Node> CalculatedPath { get; }
        void Calculate(Map map, Position currentPosition);
    }
}
