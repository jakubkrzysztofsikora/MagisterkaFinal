using System.Collections.Generic;
using System.Linq;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Graph.Pathfinding.Exceptions;
using Magisterka.Domain.Monitoring;

namespace Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies
{
    public class DijkstraStrategy : IPathfindingStrategy
    {
        private readonly IAlgorithmMonitor _monitor;

        private readonly Dictionary<Node, int> _nodeToCost = new Dictionary<Node, int>();
        private readonly Dictionary<Node, Node> _previousNodes = new Dictionary<Node, Node>();
        private Node _startingNode;
        private Map _unoptimizedGraph;

        public DijkstraStrategy(IAlgorithmMonitor monitor)
        {
            _monitor = monitor;
        }

        public IEnumerable<Node> CalculatedPath { get; private set; }

        public void Calculate(Map map, Position currentPostition)
        {
            _monitor.StartMonitoring();

            _startingNode = map.GetNodeByPosition(currentPostition);
            InitializeDistanceToNodeDictionary(map, currentPostition);
            var path = new List<Node>();
            _previousNodes.Clear();
            _unoptimizedGraph = map.DeepCopy();

            while (_unoptimizedGraph.Count > 0)
            {
                var nearestNode = GetNearestNodeInUnoptimizedGraph();
                _unoptimizedGraph.Remove(nearestNode);

                if (nearestNode.IsTargetNode)
                {
                    path = CreateOptimalPathToNode(nearestNode);
                    break;
                }

                UpdateDistances(nearestNode);
            }

            CalculatedPath = path;

            _monitor.StopMonitoring();
        }

        private void UpdateDistances(Node nearestNode)
        {
            foreach (var neighborToCost in nearestNode.Neighbors)
            {
                var neighbor = neighborToCost.Key;
                var neighborEdgeCost = neighborToCost.Value;
                var updatedDistanceToNode = _nodeToCost[nearestNode] + neighborEdgeCost.Cost;

                if (neighbor.IsBlocked || !_nodeToCost.ContainsKey(neighbor) ||
                    updatedDistanceToNode >= _nodeToCost[neighbor])
                    continue;

                _nodeToCost[neighbor] = updatedDistanceToNode;
                _previousNodes[neighbor] = nearestNode;
            }
        }

        private List<Node> CreateOptimalPathToNode(Node currentNode)
        {
            List<Node> path = new List<Node>();
            while (_previousNodes.ContainsKey(currentNode))
            {
                var nextNode = _previousNodes[currentNode];

                _monitor.MonitorPathFragment(currentNode, nextNode);
                path.Add(currentNode);
                currentNode = nextNode;
            }

            if (!path.Any() || IsPathCutOutByBlockedNodes(path))
                throw new PathToTargetDoesntExistException();

            return path;
        }

        private bool IsPathCutOutByBlockedNodes(IEnumerable<Node> path)
        {
            return !path.Last().Neighbors.ContainsKey(_startingNode);
        }

        private void InitializeDistanceToNodeDictionary(Map map, Position currentPosition)
        {
            _nodeToCost.Clear();
            _nodeToCost.Add(map.Single(node => node.Coordinates == currentPosition), 0);

            foreach (var node in map.Where(n => n.Coordinates != currentPosition))
            {
                _nodeToCost.Add(node, int.MaxValue);
            }
        }

        private Node GetNearestNodeInUnoptimizedGraph()
        {
            Node result = null;
            while (result == null)
            {
                var distanceToNodeCopy = _nodeToCost;
                var lowestDistance = distanceToNodeCopy.Min(x => x.Value);
                result =
                    _unoptimizedGraph.FirstOrDefault(
                        node => node == _nodeToCost.First(x => x.Value == lowestDistance).Key);

                if (result == null)
                {
                    distanceToNodeCopy.Remove(distanceToNodeCopy.First(x => x.Value == lowestDistance).Key);
                }
            }

            return result;
        }
    }
}