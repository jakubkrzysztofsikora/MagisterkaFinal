using System;
using System.Collections.Generic;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
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
            _pathfinderFactory = new PathfinderFactory();
            _mapFactory = new MapFactory(new Random());
            _startingPosition = new Position(Guid.NewGuid());
            _endingPosition = new Position(Guid.NewGuid());

            var listOfCoordinates = GenerateListOfCoordinates();
            _map =
                _mapFactory.GenerateMapWithProvidedCoordinates(listOfCoordinates)
                    .WithStartingPosition(_startingPosition)
                    .WithTargetPosition(_endingPosition);
        }

        [Test]
        public void ShouldFindPathBasedOnCurrentSituationWIthDijkstraAlgorithm()
        {
            //Given
            Pathfinder pathfinder = _pathfinderFactory.CreatePathfinderWithAlgorithm(ePathfindingAlgorithms.Djikstra);
            Node targetNode = _map.GetNodeByPosition(_endingPosition);

            //When
            Position currentPosition = _startingPosition;

            while (currentPosition != _endingPosition)
            {
                currentPosition = pathfinder.GetNextStep(_map, _startingPosition, _endingPosition);
            }

            //Then
            Node result = _map.GetNodeByPosition(currentPosition);
            Assert.AreEqual(targetNode, result);
        }

        private IEnumerable<Position> GenerateListOfCoordinates()
        {
            yield return _startingPosition;

            for (int i = 0; i < 18; i++)
            {
                yield return new Position(Guid.NewGuid());
            }

            yield return _endingPosition;
        } 
    }
}
