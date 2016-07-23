using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.MovementSpace.Exceptions;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Graph.MovementSpace
{
    public class MapFactory : IMapFactory
    {
        private const int DefaultNodeNumber = 20;
        private const int DefaultMaxNeighborsForNode = 5;
        private const int MinEdgeCost = 1;
        private const int MaxEdgeCost = 10;
        private const int MinNeighborNumber = 1;
        private readonly Random _randomizer;

        public MapFactory(Random randomizer)
        {
            _randomizer = randomizer;
        }

        public Map GenerateDefaultMap()
        {
            var newMap = new Map(DefaultNodeNumber);

            while (newMap.Count < newMap.MaximumNumberOfNodes)
            {
                newMap.AddIfNotExists(new Node());
            }

            GenerateNodesNeighbors(ref newMap, DefaultMaxNeighborsForNode);

            return newMap;
        }

        public Map GenerateMapWithProvidedCoordinates(IEnumerable<Position> coordinates)
        {
            var listOfCoordinates = coordinates.ToList();
            var newMap = new Map(listOfCoordinates.Count);

            foreach (var coordinate in listOfCoordinates)
            {
                newMap.AddIfNotExists(new Node
                {
                    Coordinates = coordinate
                });
            }

            GenerateNodesNeighbors(ref newMap, DefaultMaxNeighborsForNode);

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