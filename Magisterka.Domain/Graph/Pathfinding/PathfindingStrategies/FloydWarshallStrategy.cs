using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies
{
    public class FloydWarshallStrategy : IPathfindingStrategy
    {
        public IEnumerable<Node> CalculatedPath { get; private set; }

        private const long Infinity = int.MaxValue;
        private readonly Dictionary<KeyValuePair<Node, Node>, long> _distancesBetweenNodes = new Dictionary<KeyValuePair<Node, Node>, long>();
        private readonly Dictionary<KeyValuePair<Node, Node>, Node> _nextNodes = new Dictionary<KeyValuePair<Node, Node>, Node>();

        public void Calculate(Map map, Position currentPosition)
        {
            InitilizeStructures(map);

            foreach (Node middleNode in map)
            {
                foreach (Node firstNode in map.Where(firstNode => middleNode != firstNode))
                {
                    foreach (Node finalNode in map.Where(finalNode => finalNode != middleNode && finalNode != firstNode &&
                                                                      IsPathThroughOtherNodeShorter(firstNode, finalNode, middleNode)))
                    {
                        SetNewDistanceBetweenNodes(firstNode, middleNode, finalNode);
                        AddNewMiddleNodeBetweenFirstAndFinalToOptimalPath(firstNode, middleNode, finalNode);
                    }
                }
            }

            CalculatedPath = ContructPath(map, currentPosition);
        }

        private bool IsPathThroughOtherNodeShorter(Node startNode, Node targetNode, Node otherNode)
        {
            return _distancesBetweenNodes[new KeyValuePair<Node, Node>(startNode, otherNode)] +
                   _distancesBetweenNodes[new KeyValuePair<Node, Node>(otherNode, targetNode)] <
                   _distancesBetweenNodes[new KeyValuePair<Node, Node>(startNode, targetNode)];
        }

        private void AddNewMiddleNodeBetweenFirstAndFinalToOptimalPath(Node firstNode, Node middleNode, Node finalNode)
        {
            _nextNodes[new KeyValuePair<Node, Node>(firstNode, finalNode)] =
                            _nextNodes[new KeyValuePair<Node, Node>(firstNode, middleNode)];
        }

        private void SetNewDistanceBetweenNodes(Node firstNode, Node middleNode, Node finalNode)
        {
            _distancesBetweenNodes[new KeyValuePair<Node, Node>(firstNode, finalNode)] =
                            _distancesBetweenNodes[new KeyValuePair<Node, Node>(firstNode, middleNode)] +
                            _distancesBetweenNodes[new KeyValuePair<Node, Node>(middleNode, finalNode)];
        }

        private IEnumerable<Node> ContructPath(Map map, Position startingPosition)
        {
            Node startNode = map.GetNodeByPosition(startingPosition);
            Node targetNode = map.GetTargetNode();

            if (_nextNodes[new KeyValuePair<Node, Node>(startNode, targetNode)] != null)
            {
                Node current = startNode;

                while (!current.IsTargetNode)
                {
                    current = _nextNodes[new KeyValuePair<Node, Node>(current, targetNode)];
                    yield return current;
                }
            }
        }

        private void InitilizeStructures(Map map)
        {
            _distancesBetweenNodes.Clear();

            foreach (var nodeToNode in map.SelectMany(node => map.Where(anotherNode => anotherNode != node).Select(anotherNode => new KeyValuePair<Node, Node>(node, anotherNode))))
            {
                _distancesBetweenNodes[nodeToNode] = Infinity;
                _nextNodes[nodeToNode] = null;
            }

            foreach (var nodeToNode in map.SelectMany(node => node.Neighbors.Keys.Select(anotherNode => new KeyValuePair<Node, Node>(node, anotherNode))))
            {
                _distancesBetweenNodes[nodeToNode] = nodeToNode.Key.Neighbors[nodeToNode.Value].Cost;
                _nextNodes[nodeToNode] = nodeToNode.Value;
            }
        }
    }
}
