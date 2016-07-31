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

            var weightedCostFromStartToNodeMap =
                new Dictionary<Node, long>(map.ToDictionary(x => x, x => x.Coordinates == currentPosition ? 0 : Infinity));
            var heuristicCostFromStartToEndByNodeMap =
                new Dictionary<Node, long>(map.ToDictionary(x => x,
                    x =>
                        x.Coordinates == currentPosition
                            ? map.GetHeuristicScoreBetweenNodes(currentNode, targetNode)
                            : Infinity));

            while (_openSet.Count != 0)
            {
                var processedNode =
                    _openSet.First(
                        node =>
                            heuristicCostFromStartToEndByNodeMap[node] == GetMinimalHeuristicScorePresentInOpenSet(heuristicCostFromStartToEndByNodeMap));

                TransistEvaluatedNodeToClosedSet(processedNode);

                foreach (
                    var nodeToCost in
                        processedNode.Neighbors.Where(x => !_closedSet.Contains(x.Key)))
                {
                    var neighbor = nodeToCost.Key;
                    var neighborDistance = nodeToCost.Value;
                    var tentativeCostFromStartToNode = weightedCostFromStartToNodeMap[processedNode] +
                                                       (neighbor.IsBlocked ? Infinity : neighborDistance.Cost);

                    bool addedNodeToOpenSet = DiscoverNewNodeToEvaluate(neighbor);
                    if (!addedNodeToOpenSet && tentativeCostFromStartToNode >= weightedCostFromStartToNodeMap[neighbor])
                        continue;

                    _previousNodes[neighbor] = processedNode;
                    _monitor.RecordStep();
                    weightedCostFromStartToNodeMap[neighbor] = tentativeCostFromStartToNode;
                    heuristicCostFromStartToEndByNodeMap[neighbor] = weightedCostFromStartToNodeMap[neighbor] +
                                                                     map.GetHeuristicScoreBetweenNodes(neighbor,
                                                                         targetNode);
                }
            }
            
            List<Node> path = CreateOptimalPath(currentNode, targetNode).ToList();
            CalculatedPath = path;

            _monitor.StopMonitoring();
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