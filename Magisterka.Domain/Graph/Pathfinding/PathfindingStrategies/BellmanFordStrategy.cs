using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Graph.Pathfinding.Exceptions;

namespace Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies
{
    public class BellmanFordStrategy : IPathfindingStrategy
    {
        public IEnumerable<Node> CalculatedPath { get; private set; }

        private readonly Dictionary<Node, long> _nodesToCosts = new Dictionary<Node, long>();
        private readonly Dictionary<Node, Node> _previousNodes = new Dictionary<Node, Node>();
        private const long Infinity = int.MaxValue;

        public void Calculate(Map map, Position currentPosition)
        {
            Node currentNode = map.GetNodeByPosition(currentPosition);
            Node targetNode = map.SingleOrDefault(node => node.IsTargetNode);

            List<Edge> edges = map.GetAllEdgeCosts().ToList();

            InitilizeDataStructures(map, currentNode);

            RelaxMapEdges(map, edges);

            CheckForNegativeWeights(edges);
            
            CalculatedPath = CreateOptimalPath(currentNode, targetNode);
        }

        private void CheckForNegativeWeights(List<Edge> edges)
        {
            IterateEdgesAndPerformAction(edges, (edge, node1, node2) =>
            {
                throw new NegativeWeightCycleException();
            });
        }

        private void RelaxMapEdges(Map nodes, List<Edge> edges)
        {
            for (int i = 1; i < nodes.Count - 1; ++i)
            {
                IterateEdgesAndPerformAction(edges, (edge, node1, node2) =>
                {
                    _nodesToCosts[node2] = _nodesToCosts[node1] + edge.Cost;
                    _previousNodes[node2] = node1;
                });
            }
        }

        private void IterateEdgesAndPerformAction(List<Edge> edges, Action<Edge, Node, Node> action)
        {
            foreach (var edge in edges)
            {
                var node1 = edge.NodesConnected.Value;
                var node2 = edge.NodesConnected.Key;

                if (_nodesToCosts[node2] > _nodesToCosts[node1] + edge.Cost && !node2.IsBlocked)
                {
                    action.Invoke(edge, node1, node2);
                }
            }
        }

        private void InitilizeDataStructures(Map map, Node currentNode)
        {
            _nodesToCosts.Clear();
            _previousNodes.Clear();

            foreach (var node in map.Where(node => node != currentNode))
            {
                _nodesToCosts[node] = Infinity;
                _previousNodes[node] = null;
            }

            _nodesToCosts[currentNode] = 0;
        }

        private IEnumerable<Node> CreateOptimalPath(Node currentNode, Node targetNode)
        {
            while (targetNode != currentNode)
            {
                yield return targetNode;
                targetNode = _previousNodes[targetNode];
            }
        }
    }
}
