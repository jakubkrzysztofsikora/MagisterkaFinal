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
                                                                      _distancesBetweenNodes[new KeyValuePair<Node, Node>(firstNode, middleNode)] +
                                                                      _distancesBetweenNodes[new KeyValuePair<Node, Node>(middleNode, finalNode)] <
                                                                      _distancesBetweenNodes[new KeyValuePair<Node, Node>(firstNode, finalNode)]))
                    {
                        _distancesBetweenNodes[new KeyValuePair<Node, Node>(firstNode, finalNode)] =
                            _distancesBetweenNodes[new KeyValuePair<Node, Node>(firstNode, middleNode)] +
                            _distancesBetweenNodes[new KeyValuePair<Node, Node>(middleNode, finalNode)];

                        _nextNodes[new KeyValuePair<Node, Node>(firstNode, finalNode)] =
                            _nextNodes[new KeyValuePair<Node, Node>(firstNode, middleNode)];
                    }
                }
            }

            CalculatedPath = ContructPath(map, currentPosition);
        }

        private IEnumerable<Node> ContructPath(Map map, Position startingPosition)
        {
            Node startNode = map.GetNodeByPosition(startingPosition);
            Node targetNode = map.GetTargetNode();

            if (_nextNodes[new KeyValuePair<Node, Node>(startNode, targetNode)] != null)
            {
                Node current = startNode;
                yield return current;

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
                _distancesBetweenNodes[nodeToNode] = nodeToNode.Key.Neighbors[nodeToNode.Value].Value;
                _nextNodes[nodeToNode] = nodeToNode.Value;
            }
        }
    }
}
