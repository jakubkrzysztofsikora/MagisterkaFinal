using System;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using NUnit.Framework;

namespace MagisterkaTests
{
    [TestFixture]
    public class MonitoringTests
    {
        private MapFactory _mapFactory;
        private Map _map;
        private Position _startingPosition;
        private Position _endingPosition;

        [OneTimeSetUp]
        public void Init()
        {
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
        public void ShouldLogAlgorithmPerformance(ePathfindingAlgorithms algorithm)
        {
            //Given
            AlgorithmMonitor monitor = new AlgorithmMonitor();
            PathfinderFactory factory = new PathfinderFactory(monitor);
            Pathfinder pathfinder = factory.CreatePathfinderWithAlgorithm(algorithm);

            //When
            pathfinder.GetNextStep(_map, _startingPosition);

            //Then
            Assert.AreNotEqual(0, monitor.PerformanceResults.TimeOfComputing.TotalSeconds);
            Assert.AreNotEqual(0, monitor.PerformanceResults.AverageMemoryUsageInBytes);
            Assert.AreNotEqual(0, monitor.PerformanceResults.PeakMemoryUsageInBytes);
            Assert.AreNotEqual(0, monitor.PerformanceResults.AverageProcessorUsageInPercents);
            Assert.AreNotEqual(0, monitor.PerformanceResults.PeakProcessorUsageInPercents);
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldLogPathDetailsAfterOneStepOfPathfinding(ePathfindingAlgorithms algorithm)
        {
            //Given
            AlgorithmMonitor monitor = new AlgorithmMonitor();
            PathfinderFactory factory = new PathfinderFactory(monitor);
            Pathfinder pathfinder = factory.CreatePathfinderWithAlgorithm(algorithm);

            //When
            pathfinder.GetNextStep(_map, _startingPosition);

            //Then
            Assert.IsNotNull(monitor.PathDetails);
            Assert.AreEqual(1, monitor.PathDetails.StepsTaken);
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldLogPathDetailsAfterManyStepsOfPathfinding(ePathfindingAlgorithms algorithm)
        {
            //Given
            AlgorithmMonitor monitor = new AlgorithmMonitor();
            PathfinderFactory factory = new PathfinderFactory(monitor);
            Pathfinder pathfinder = factory.CreatePathfinderWithAlgorithm(algorithm);
            Position currentPosition = _startingPosition;

            //When
            while (currentPosition != _endingPosition)
                currentPosition = pathfinder.GetNextStep(_map, currentPosition);

            //Then
            Assert.IsNotNull(monitor.PathDetails);
            Assert.AreNotEqual(1, monitor.PathDetails.StepsTaken);
            Assert.AreNotEqual(0, monitor.PathDetails.StepsTaken);
        }
    }
}
