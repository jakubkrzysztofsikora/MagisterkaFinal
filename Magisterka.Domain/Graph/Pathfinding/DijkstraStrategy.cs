using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies;

namespace Magisterka.Domain.Graph.Pathfinding
{
    public class DijkstraStrategy : IPathfindingStrategy
    {
        private readonly Dictionary<Node, int> _distanceToNode = new Dictionary<Node, int>();
        private Map _unoptimizedGraph;

        public Position Calculate(Map map)
        {
            _distanceToNode.Add(map.Single(node => node.IsStartingNode), 0);

            foreach (var node in map.Where(n => !n.IsStartingNode))
            {
                _distanceToNode.Add(node, int.MaxValue);
            }

            _unoptimizedGraph = map.DeepCopy();

            while (_unoptimizedGraph.Count > 0)
            {
                var nearestNode = GetNearestNodeInUnoptimizedGraph();
                _unoptimizedGraph.Remove(nearestNode);

                foreach (var nodeToEdgeCost in nearestNode.Neighbors.Where(neighbor => _unoptimizedGraph.Contains(neighbor.Key)))
                {
                    var alt = _distanceToNode[nearestNode] + nodeToEdgeCost.Value.Value;

                    if (alt < _distanceToNode[nodeToEdgeCost.Key])
                    {
                        _distanceToNode[nodeToEdgeCost.Key] = alt;
                    }
                }
            }

            return _distanceToNode.First(dist => dist.Value == _distanceToNode.Min(x => x.Value)).Key.Coordinates;
        }

        private Node GetNearestNodeInUnoptimizedGraph()
        {
            Node result = null;
            while (result == null)
            {
                var distanceToNodeCopy = _distanceToNode;
                int lowestDistance = distanceToNodeCopy.Min(x => x.Value);
                result = _unoptimizedGraph.FirstOrDefault(node => node == _distanceToNode.First(x => x.Value == lowestDistance).Key);

                if (result == null)
                {
                    distanceToNodeCopy.Remove(distanceToNodeCopy.First(x => x.Value == lowestDistance).Key);
                }
            }

            return result;
        }
    }
}
