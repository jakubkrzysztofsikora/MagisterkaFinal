using System;
using System.Collections.Generic;
using System.Linq;
using GraphX.PCL.Logic.Helpers;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Graph.MovementSpace
{
    public static class MapSpecification
    {
        public static Map WithStartingPosition(this Map map, Position startingPosition)
        {
            map.ClearStartingPosition();

            var startingNode = map.GetNodeByPosition(startingPosition);
            startingNode.IsStartingNode = true;
            startingNode.IsBlocked = false;

            return map;
        }

        public static Map WithTargetPosition(this Map map, Position endingPosition)
        {
            map.ClearTargetPosition();

            var endingNode = map.GetNodeByPosition(endingPosition);
            endingNode.IsTargetNode = true;
            endingNode.IsBlocked = false;

            return map;
        }

        public static Map WithNoDefinedPositions(this Map map)
        {
            foreach (var node in map.Where(node => node.IsStartingNode || node.IsTargetNode))
            {
                node.IsStartingNode = false;
                node.IsTargetNode = false;
            }

            return map;
        }

        public static Map WithGridPositions(this Map map)
        {
            Node firstNode = map.First();
            firstNode.Coordinates.X = 0;
            firstNode.Coordinates.Y = 0;
            GenerateNodesCoordinates(firstNode);

            return map;
        }

        public static Map Validated(this Map map)
        {
            MapValidator validator = new MapValidator(map);
            validator.ValidateNodes();
            return map;
        }

        public static Map WithRandomBlockedNodes(this Map map, Random randomizer)
        {
            foreach (var node in map.Where(node => !node.IsStartingNode && !node.IsTargetNode))
            {
                node.IsBlocked = ShouldNodeBeBlocked(node) && RandomizeBlockedStatus(randomizer);
            }

            return map;
        }

        public static void ClearStartingPosition(this Map map)
        {
            Node startingNode = map.SingleOrDefault(node => node.IsStartingNode);

            if (startingNode != null)
                startingNode.IsStartingNode = false;
        }

        public static void ClearTargetPosition(this Map map)
        {
            Node targetNode = map.SingleOrDefault(node => node.IsTargetNode);

            if (targetNode != null)
                targetNode.IsTargetNode = false;
        }

        private static bool ShouldNodeBeBlocked(Node node)
        {
            return node.Neighbors.All(neighbor => !neighbor.Key.IsBlocked);
        }
        private static void GenerateNodesCoordinates(Node node, int xPosition = 0)
        {
            List<Node> neighbors = node.Neighbors.Keys.Where(x => !x.IsOnTheGrid()).ToList();
            int yPosition = node.Coordinates.Y.Value + 1;

            foreach (Node neighbor in neighbors)
            {
                neighbor.Coordinates.Y = yPosition;
                neighbor.Coordinates.X = xPosition;
                ++xPosition;
            }

            xPosition = 0;
            foreach (var neighbor in neighbors)
            {
                GenerateNodesCoordinates(neighbor, xPosition);
                xPosition += neighbor.Neighbors.Count;
            }
        }

        private static bool RandomizeBlockedStatus(Random randomizer, int maxOfBlocked = 3)
        {
            return randomizer.Next(0, maxOfBlocked) == 0;
        }
    }
}
