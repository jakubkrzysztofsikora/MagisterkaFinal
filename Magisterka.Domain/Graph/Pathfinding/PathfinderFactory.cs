using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies;
using Magisterka.Domain.Monitoring;

namespace Magisterka.Domain.Graph.Pathfinding
{
    public class PathfinderFactory : IPathfinderFactory
    {
        private readonly IAlgorithmMonitor _monitor;

        public PathfinderFactory(IAlgorithmMonitor monitor)
        {
            _monitor = monitor;
        }

        public Pathfinder CreatePathfinderWithAlgorithm(ePathfindingAlgorithms algorithm)
        {
            switch (algorithm)
            {
                case ePathfindingAlgorithms.Djikstra:
                    return new Pathfinder(new DijkstraStrategy(_monitor));
                case ePathfindingAlgorithms.BellmanFord:
                    return new Pathfinder(new BellmanFordStrategy(_monitor));
                case ePathfindingAlgorithms.AStar:
                    return new Pathfinder(new AStarStrategy(_monitor));
                case ePathfindingAlgorithms.FloydWarshall:
                    return new Pathfinder(new FloydWarshallStrategy(_monitor));
                case ePathfindingAlgorithms.Johnson:
                    return new Pathfinder(new JohnsonStrategy(_monitor));
                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null);
            }
        }
    }
}
