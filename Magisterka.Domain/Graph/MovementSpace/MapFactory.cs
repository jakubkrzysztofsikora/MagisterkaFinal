using System;
using System.Collections.Generic;
using System.Linq;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Graph.MovementSpace
{
    public class MapFactory : IMapFactory
    {
        private const int DefaultNodeNumber = 20;
        private const int DefaultMaxNeighborsForNode = 4;
        private const int MinEdgeCost = 1;
        private const int MaxEdgeCost = 10;
        private const int MinNeighborNumber = 1;

        private const string NodeNamePrefix = "Node";

        private readonly Random _randomizer;

        public MapFactory(Random randomizer)
        {
            _randomizer = randomizer;
        }

        public Map GenerateDefaultMap()
        {
            return GenerateMap(DefaultNodeNumber, DefaultMaxNeighborsForNode);
        }

        public Map GenerateMapWithProvidedCoordinates(IEnumerable<Position> coordinates)
        {
            var listOfCoordinates = coordinates.ToList();
            var newMap = new Map(listOfCoordinates.Count);
            var nodeCounter = 0;

            foreach (var coordinate in listOfCoordinates)
            {
                ++nodeCounter;
                newMap.AddIfNotExists(new Node($"{NodeNamePrefix} {nodeCounter}")
                {
                    Coordinates = coordinate
                });
            }

            GenerateNodesNeighbors(ref newMap, DefaultMaxNeighborsForNode);

            return newMap;
        }

        public Map GenerateMap(int numberOfNodes, int maxNumberOfNeighborsPerNode)
        {
            var newMap = new Map(numberOfNodes);
            var nodeCounter = 0;

            while (newMap.Count < newMap.MaximumNumberOfNodes)
            {
                ++nodeCounter;
                newMap.AddIfNotExists(new Node($"{NodeNamePrefix} {nodeCounter}"));
            }

            GenerateNodesNeighbors(ref newMap, maxNumberOfNeighborsPerNode);

            return newMap;
        }

        private void GenerateNodesNeighbors(ref Map map, int maxNumberOfNeighbors)
        {
            foreach (var node in map)
            {
                var numberOfNeighbors = _randomizer.Next(MinNeighborNumber, maxNumberOfNeighbors);
                var newNeighbors =
                    map.Where(
                        otherNode =>
                            otherNode != node && !otherNode.IsNeighborWith(node) &&
                            otherNode.Neighbors.Count < maxNumberOfNeighbors)
                        .Take(numberOfNeighbors - node.Neighbors.Count).Concat(node.Neighbors.Keys).Distinct();

                node.Neighbors = newNeighbors.ToDictionary(x => x, x => new Edge
                {
                    Cost = _randomizer.Next(MinEdgeCost, MaxEdgeCost),
                    NodesConnected = new KeyValuePair<Node, Node>(node, x)
                });

                foreach (var neighbor in node.Neighbors.Where(neighbor => !neighbor.Key.IsNeighborWith(node)))
                {
                    neighbor.Key.Neighbors.Add(node, neighbor.Value);
                }
            }
        }
    }
}