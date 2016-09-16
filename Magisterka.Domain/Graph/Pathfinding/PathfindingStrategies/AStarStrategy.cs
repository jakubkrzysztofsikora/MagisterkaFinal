using System.Collections.Generic;
using System.Linq;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Graph.Pathfinding.Exceptions;
using Magisterka.Domain.Monitoring;

namespace Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies
{
    public class AStarStrategy : IPathfindingStrategy
    {
        private const long Infinity = int.MaxValue;
        private readonly List<Node> _closedSet = new List<Node>();

        private readonly IAlgorithmMonitor _monitor;
        private readonly List<Node> _openSet = new List<Node>();

        private readonly Dictionary<Node, Node> _previousNodes = new Dictionary<Node, Node>();

        public AStarStrategy(IAlgorithmMonitor monitor)
        {
            _monitor = monitor;
        }

        public IEnumerable<Node> CalculatedPath { get; private set; }

        public void Calculate(Map map, Position currentPosition)
        {
            _monitor.StartMonitoring();

            var currentNode = map.GetNodeByPosition(currentPosition);
            var targetNode = map.GetTargetNode();

            InitilizeSets(currentNode);

            var weightedCostFromStartToNodeMap = InitilizeDictionaryNodeToWeightDistanceFromStart(map, currentPosition);
            var heuristicCostFromNodeToTargetMap = InitilizeDictionaryNodeToHeuristicDistanceToTarget(map, currentNode,
                targetNode);

            while (_openSet.Count != 0)
            {
                var processedNode = GetNodeFromOpenSetWithMinimalHeuristicScore(heuristicCostFromNodeToTargetMap);

                TransistEvaluatedNodeToClosedSet(processedNode);

                foreach (var nodeToCost in GetNeighborPairsNotIncludedInClosedSet(processedNode.Neighbors))
                {
                    var neighbor = nodeToCost.Key;
                    var neighborDistance = nodeToCost.Value;
                    var tentativeCostFromStartToNode = weightedCostFromStartToNodeMap[processedNode] +
                                                       (neighbor.IsBlocked ? Infinity : neighborDistance.Cost);

                    _monitor.RecordNodeProcessed(neighbor);
                    bool addedNodeToOpenSet = DiscoverNewNodeToEvaluate(neighbor);
                    if (!addedNodeToOpenSet && tentativeCostFromStartToNode >= weightedCostFromStartToNodeMap[neighbor])
                        continue;

                    _previousNodes[neighbor] = processedNode;
                    weightedCostFromStartToNodeMap[neighbor] = tentativeCostFromStartToNode;
                    heuristicCostFromNodeToTargetMap[neighbor] = weightedCostFromStartToNodeMap[neighbor] +
                                                                     map.GetHeuristicScoreBetweenNodes(neighbor,
                                                                         targetNode);
                }
            }
            
            ConstructPath(currentNode, targetNode);

            _monitor.StopMonitoring();
        }

        private Dictionary<Node, Edge> GetNeighborPairsNotIncludedInClosedSet(IDictionary<Node, Edge> neighborCollection)
        {
            return neighborCollection.Where(x => !_closedSet.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
        }

        private Node GetNodeFromOpenSetWithMinimalHeuristicScore(Dictionary<Node, long> heuristicCostFromNodeToTargetMap)
        {
            return _openSet.First(
                node =>
                    heuristicCostFromNodeToTargetMap[node] ==
                    GetMinimalHeuristicScorePresentInOpenSet(heuristicCostFromNodeToTargetMap));
        }

        private Dictionary<Node, long> InitilizeDictionaryNodeToHeuristicDistanceToTarget(Map graph, Node currentNode, Node targetNode)
        {
            return new Dictionary<Node, long>(graph.ToDictionary(x => x,
                x =>
                    x.Coordinates == currentNode.Coordinates
                        ? graph.GetHeuristicScoreBetweenNodes(currentNode, targetNode)
                        : Infinity));
        }

        private Dictionary<Node, long> InitilizeDictionaryNodeToWeightDistanceFromStart(Map graph, Position startPosition)
        {
            return
                new Dictionary<Node, long>(graph.ToDictionary(x => x, x => x.Coordinates == startPosition ? 0 : Infinity));
        }

        private void ConstructPath(Node currentNode, Node targetNode)
        {
            List<Node> path = CreateOptimalPath(currentNode, targetNode).ToList();
            CalculatedPath = path;
        }

        private bool DiscoverNewNodeToEvaluate(Node nodeToEvaluate)
        {
            if (_openSet.Contains(nodeToEvaluate) || nodeToEvaluate.IsBlocked) return false;
            _openSet.Add(nodeToEvaluate);
            return true;
        }

        private void TransistEvaluatedNodeToClosedSet(Node evaluatedNode)
        {
            _openSet.Remove(evaluatedNode);
            _closedSet.Add(evaluatedNode);
        }

        private long GetMinimalHeuristicScorePresentInOpenSet(Dictionary<Node, long> nodeToHeuristics)
        {
            return nodeToHeuristics.Where(x => _openSet.Contains(x.Key)).Min(x => x.Value);
        }

        private void InitilizeSets(Node startingNode)
        {
            _openSet.Clear();
            _closedSet.Clear();
            _openSet.Add(startingNode);
        }

        private IEnumerable<Node> CreateOptimalPath(Node currentNode, Node targetNode)
        {
            while (targetNode != currentNode)
            {
                Node nextNode;
                try
                {
                    nextNode = _previousNodes[targetNode];
                }
                catch (KeyNotFoundException)
                {
                    throw new PathToTargetDoesntExistException();
                }
                
                _monitor.MonitorPathFragment(targetNode, nextNode);

                yield return targetNode;
                targetNode = nextNode;
            }
        }
    }
}