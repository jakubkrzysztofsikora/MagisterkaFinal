using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies
{
    public class AStarStrategy : IPathfindingStrategy
    {
        public IEnumerable<Node> CalculatedPath { get; private set; }

        private readonly Dictionary<Node, Node> _previousNodes = new Dictionary<Node, Node>();
        private const long Infinity = int.MaxValue;
        public void Calculate(Map map, Position currentPosition)
        {
            List<Node> closedSet = new List<Node>();
            Node currentNode = map.GetNodeByPosition(currentPosition);
            Node targetNode = map.GetTargetNode();
            List<Node> openSet = new List<Node>
            {
                currentNode
            };

            Dictionary<Node, long> generalCostFromStartToNodeMap = new Dictionary<Node, long>(map.ToDictionary(x => x, x => x.Coordinates == currentPosition ? 0 : Infinity));
            Dictionary<Node, long> heuristicCostFromStartToEndByNodeMap = new Dictionary<Node, long>(map.ToDictionary(x => x, x => x.Coordinates == currentPosition ? map.GetHeuristicScoreBetweenNodes(currentNode, targetNode) : Infinity));

            while (openSet.Count != 0)
            {
                Node processedNode = openSet.First(node => heuristicCostFromStartToEndByNodeMap[node] == heuristicCostFromStartToEndByNodeMap.Where(x => openSet.Contains(x.Key)).Min(x => x.Value));

                if (processedNode != null && processedNode.IsTargetNode)
                {
                    CalculatedPath = new List<Node>
                    {
                        currentNode
                    };
                }

                if (processedNode != null)
                {
                    openSet.Remove(processedNode);
                    closedSet.Add(processedNode);
                }

                foreach (var nodeToCost in processedNode.Neighbors.Where(x => !closedSet.Contains(x.Key) && !x.Key.IsBlocked))
                {
                    Node neighbor = nodeToCost.Key;
                    EdgeCost neighborDistance = nodeToCost.Value;
                    long tentativeCostFromStartToNode = generalCostFromStartToNodeMap[processedNode] + neighborDistance.Value;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    else if (tentativeCostFromStartToNode >= generalCostFromStartToNodeMap[neighbor])
                        continue;

                    _previousNodes[neighbor] = processedNode;
                    generalCostFromStartToNodeMap[neighbor] = tentativeCostFromStartToNode;
                    heuristicCostFromStartToEndByNodeMap[neighbor] = generalCostFromStartToNodeMap[neighbor] +
                                                                     map.GetHeuristicScoreBetweenNodes(neighbor,
                                                                         targetNode);
                }
            }

            CalculatedPath = CreateOptimalPath(currentNode, targetNode);
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
