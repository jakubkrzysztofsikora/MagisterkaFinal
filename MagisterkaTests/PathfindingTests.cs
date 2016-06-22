using System;
using System.Collections.Generic;
using System.Linq;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Monitoring;
using NUnit.Framework;

namespace MagisterkaTests
{
    [TestFixture]
    public class PathfindingTests
    {
        private PathfinderFactory _pathfinderFactory;
        private MapFactory _mapFactory;
        private Map _map;
        private Position _startingPosition;
        private Position _endingPosition;

        [OneTimeSetUp]
        public void Init()
        {
            _pathfinderFactory = new PathfinderFactory(new AlgorithmMonitor(new PerformanceMonitor()));
            _mapFactory = new MapFactory(new Random());
            _startingPosition = new Position(Guid.NewGuid());
            _endingPosition = new Position(Guid.NewGuid());

            var listOfCoordinates = TestCommon.GenerateListOfCoordinates(_startingPosition, _endingPosition);
            _map =
                _mapFactory.GenerateMapWithProvidedCoordinates(listOfCoordinates)
                    .WithStartingPosition(_startingPosition)
                    .WithTargetPosition(_endingPosition)
                    .WithGridPositions()
                    .Validated();
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldFindPathBasedOnCurrentSituation(ePathfindingAlgorithms algorithm)
        {
            //Given
            Pathfinder pathfinder = _pathfinderFactory.CreatePathfinderWithAlgorithm(algorithm);
            Node targetNode = _map.GetNodeByPosition(_endingPosition);

            //When
            Position currentPosition = _startingPosition;

            while (currentPosition != _endingPosition)
            {
                currentPosition = pathfinder.GetNextStep(_map, currentPosition);
            }

            //Then
            Node result = _map.GetNodeByPosition(currentPosition);
            Assert.AreEqual(targetNode, result);
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldNotChooseBlockedNodeForTheNExtStep(ePathfindingAlgorithms algorithm)
        {
            //Given
            Pathfinder pathfinder = _pathfinderFactory.CreatePathfinderWithAlgorithm(algorithm);
            Map map = _map.WithRandomBlockedNodes(new Random());

            //When
            Position currentPosition = _startingPosition;

            while (currentPosition != _endingPosition)
            {
                currentPosition = pathfinder.GetNextStep(map, currentPosition);

                //Then
                Node result = map.GetNodeByPosition(currentPosition);
                Assert.False(result.IsBlocked);
            }
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldFindOPTIMALPathNotJustPathFromAToB(ePathfindingAlgorithms algorithm)
        {
            //Given
            Pathfinder pathfinder = _pathfinderFactory.CreatePathfinderWithAlgorithm(algorithm);
            Position positionOfTheMoreOptimalNode = new Position(Guid.NewGuid())
            {
                X = 1,
                Y = 1
            };
            Map map = CreateTestMap(positionOfTheMoreOptimalNode);

            //When
            Position currentPosition = _startingPosition;

            IEnumerable<Node> result = pathfinder.GetOptimalPath(map, currentPosition);

            //Then
            Assert.True(result.Any(node => node.Coordinates == positionOfTheMoreOptimalNode));
        }

        private Map CreateTestMap(Position positionToInjectInTheMiddle)
        {
            Node nodeA = new Node();
            Node nodeB = new Node();
            Node nodeC = new Node();
            Node nodeD = new Node();

            nodeA.IsStartingNode = true;
            nodeD.IsTargetNode = true;

            nodeA.Coordinates = _startingPosition;
            nodeA.Coordinates.X = 0;
            nodeA.Coordinates.Y = 0;
            nodeD.Coordinates = _endingPosition;
            nodeA.Coordinates.X = 0;
            nodeA.Coordinates.Y = 2;
            nodeB.Coordinates = new Position(Guid.NewGuid())
            {
                X = 0,
                Y = 1
            };
            nodeC.Coordinates = positionToInjectInTheMiddle;

            nodeA.Neighbors.Add(nodeB, new Edge { Cost = 1, NodesConnected = new KeyValuePair<Node, Node>(nodeA, nodeB)});
            nodeA.Neighbors.Add(nodeC, new Edge { Cost = 2, NodesConnected = new KeyValuePair<Node, Node>(nodeA, nodeC) });

            nodeB.Neighbors.Add(nodeA, new Edge { Cost = 1, NodesConnected = new KeyValuePair<Node, Node>(nodeB, nodeA) });
            nodeB.Neighbors.Add(nodeD, new Edge { Cost = 10, NodesConnected = new KeyValuePair<Node, Node>(nodeB, nodeD) });

            nodeC.Neighbors.Add(nodeA, new Edge { Cost = 2, NodesConnected = new KeyValuePair<Node, Node>(nodeC, nodeA) });
            nodeC.Neighbors.Add(nodeD, new Edge { Cost = 5, NodesConnected = new KeyValuePair<Node, Node>(nodeC, nodeD) });

            nodeD.Neighbors.Add(nodeB, new Edge { Cost = 10, NodesConnected = new KeyValuePair<Node, Node>(nodeD, nodeB) });
            nodeD.Neighbors.Add(nodeC, new Edge { Cost = 5, NodesConnected = new KeyValuePair<Node, Node>(nodeD, nodeC) });

            return new Map(new List<Node> {nodeA, nodeB, nodeC, nodeD});
        }
    }
}
