using System.Collections.Generic;
using System.Linq;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Graph.Pathfinding.Exceptions;
using Magisterka.Domain.Monitoring;

namespace Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies
{
    public class BellmanFordStrategy : IPathfindingStrategy
    {
        private const long Infinity = int.MaxValue;

        private readonly IAlgorithmMonitor _monitor;

        private readonly Dictionary<Node, long> _nodesToCosts = new Dictionary<Node, long>();
        private readonly Dictionary<Node, Node> _previousNodes = new Dictionary<Node, Node>();

        public BellmanFordStrategy(IAlgorithmMonitor monitor)
        {
            _monitor = monitor;
        }

        public IEnumerable<Node> CalculatedPath { get; private set; }

        public void Calculate(Map map, Position currentPosition)
        {
            _monitor.StartMonitoring();

            Node currentNode = map.GetNodeByPosition(currentPosition);
            Node targetNode = map.SingleOrDefault(node => node.IsTargetNode);

            List<Edge> edges = map.GetAllEdges().ToList();

            InitilizeDataStructures(map, currentNode);

            RelaxMapEdges(map, edges);
            
            List<Node> path = CreateOptimalPath(currentNode, targetNode).ToList();
            CalculatedPath = path;

            _monitor.StopMonitoring();
        }

        private void RelaxMapEdges(Map nodes, List<Edge> edges)
        {
            for (int i = 1; i < nodes.Count - 1; ++i)
            {
                foreach (
                    var edge in
                        edges.Where(edge => !edge.NodesConnected.Key.IsBlocked && !edge.NodesConnected.Value.IsBlocked))
                {
                    var node1 = edge.NodesConnected.Key;
                    var node2 = edge.NodesConnected.Value;

                    RelaxEdge(node1, node2, edge.Cost);
                    RelaxEdge(node2, node1, edge.Cost);
                }
            }
        }

        private void RelaxEdge(Node incomingNode, Node outcomingNode, int cost)
        {
            if (_nodesToCosts[outcomingNode] > _nodesToCosts[incomingNode] + cost)
            {
                _nodesToCosts[outcomingNode] = _nodesToCosts[incomingNode] + cost;
                _previousNodes[outcomingNode] = incomingNode;
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

        private IEnumerable<Node> CreateOptimalPath(Node startNode, Node targetNode)
        {
            while (targetNode != startNode)
            {
                var nextNode = _previousNodes[targetNode];
                if (nextNode == null)
                    throw new PathToTargetDoesntExistException();


                _monitor.MonitorPathFragment(targetNode, nextNode);

                yield return targetNode;
                targetNode = nextNode;
            }
        }
    }
}
