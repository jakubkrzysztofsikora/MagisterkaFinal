using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies;

namespace Magisterka.Domain.Graph.Pathfinding
{
    public class PathfinderFactory
    {
        public Pathfinder CreatePathfinderWithAlgorithm(ePathfindingAlgorithms algorithm)
        {
            switch (algorithm)
            {
                case ePathfindingAlgorithms.Djikstra:
                    return new Pathfinder(new DijkstraStrategy());
                case ePathfindingAlgorithms.BellmanFord:
                    return new Pathfinder(new BellmanFordStrategy());
                case ePathfindingAlgorithms.AStar:
                    throw new NotImplementedException();
                case ePathfindingAlgorithms.FloydWarshall:
                    throw new NotImplementedException();
                case ePathfindingAlgorithms.Johnson:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null);
            }
        }
    }
}
