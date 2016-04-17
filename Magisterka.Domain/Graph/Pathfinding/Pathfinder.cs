using System.Collections.Generic;
using System.Linq;
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

        public Position GetNextStep(Map activeMap, Position currentPosition)
        {
            _strategy.Calculate(activeMap, currentPosition);

            return _strategy.CalculatedPath.Select(node => node.Coordinates).Last();
        }

        public List<Node> GetOptimalPath(Map activeMap, Position currentPosition)
        {
            _strategy.Calculate(activeMap, currentPosition);

            return _strategy.CalculatedPath;
        } 
    }
}
