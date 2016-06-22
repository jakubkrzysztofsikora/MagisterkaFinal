using System.Collections.Generic;
using System.Linq;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Graph.Pathfinding.Exceptions;
using Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies;
using Magisterka.Domain.Monitoring;

namespace Magisterka.Domain.Graph.Pathfinding
{
    public class Pathfinder
    {
        private readonly IPathfindingStrategy _strategy;
        private readonly AlgorithmMonitor _monitor;

        public Pathfinder(IPathfindingStrategy strategy, AlgorithmMonitor monitor)
        {
            _strategy = strategy;
            _monitor = monitor;
        }

        public Position GetNextStep(Map activeMap, Position currentPosition)
        {
            Validate(activeMap, currentPosition);

            _monitor.StartMonitoring();
            _strategy.Calculate(activeMap, currentPosition);
            _monitor.StopMonitoring();

            var path = _strategy.CalculatedPath.ToList();
            bool isPathStartingWithTargetNode = path.Count > 1 && path.First().IsTargetNode;

            return isPathStartingWithTargetNode 
                ? path.Select(node => node.Coordinates).Last() 
                : path.Select(node => node.Coordinates).First();
        }

        public IEnumerable<Node> GetOptimalPath(Map activeMap, Position currentPosition)
        {
            Validate(activeMap, currentPosition);
            _strategy.Calculate(activeMap, currentPosition);

            return _strategy.CalculatedPath;
        }

        private void Validate(Map activeMap, Position currentPosition)
        {
            if (activeMap.Any(node => node.IsStartingNode && node.IsTargetNode) || activeMap.GetNodeByPosition(currentPosition).IsTargetNode)
            {
                throw new StartIsTargetPositionException();
            }
        }
    }
}
