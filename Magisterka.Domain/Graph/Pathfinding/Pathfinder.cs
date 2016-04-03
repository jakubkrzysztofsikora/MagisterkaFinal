using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies;

namespace Magisterka.Domain.Graph.Pathfinding
{
    public class Pathfinder
    {
        private readonly IPathfindingStrategy _strategy;

        public Pathfinder(IPathfindingStrategy strategy)
        {
            _strategy = strategy;
        }

        public Position GetNextStep(Map activeMap, Position currentPosition, Position targetPosition)
        {
            return _strategy.Calculate(activeMap);
        }
    }
}
