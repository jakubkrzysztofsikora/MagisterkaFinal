using System;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using Magisterka.Domain.Monitoring.Performance;
using Magisterka.Domain.Monitoring.Quality;
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
        public void ShouldLogAlgorithmTimeOfCumputing(ePathfindingAlgorithms algorithm)
        {
            //Given
            AlgorithmMonitor monitor = new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry());
            PathfinderFactory factory = new PathfinderFactory(monitor);
            Pathfinder pathfinder = factory.CreatePathfinderWithAlgorithm(algorithm);

            //When
            pathfinder.GetNextStep(_map, _startingPosition);

            //Then
            Assert.AreNotEqual(0, monitor.PerformanceResults.TimeOfComputing.TotalSeconds);
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldLogAlgorithmMemoryUsage(ePathfindingAlgorithms algorithm)
        {
            //Given
            AlgorithmMonitor monitor = new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry());
            PathfinderFactory factory = new PathfinderFactory(monitor);
            Pathfinder pathfinder = factory.CreatePathfinderWithAlgorithm(algorithm);

            //When
            pathfinder.GetNextStep(_map, _startingPosition);

            //Then
            Assert.AreNotEqual(0, monitor.PerformanceResults.AverageMemoryUsageInBytes);
            Assert.AreNotEqual(0, monitor.PerformanceResults.PeakMemoryUsageInBytes);
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldLogAlgorithmProcessorUsage(ePathfindingAlgorithms algorithm)
        {
            //Given
            AlgorithmMonitor monitor = new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry());
            PathfinderFactory factory = new PathfinderFactory(monitor);
            Pathfinder pathfinder = factory.CreatePathfinderWithAlgorithm(algorithm);

            //When
            pathfinder.GetNextStep(_map, _startingPosition);

            //Then
            Assert.AreNotEqual(0, monitor.PerformanceResults.AverageProcessorUsageInPercents);
            Assert.AreNotEqual(0, monitor.PerformanceResults.PeakProcessorUsageInPercents);
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldLogStepsTakenAfterOneStepOfPathfinding(ePathfindingAlgorithms algorithm)
        {
            //Given
            AlgorithmMonitor monitor = new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry());
            PathfinderFactory factory = new PathfinderFactory(monitor);
            Pathfinder pathfinder = factory.CreatePathfinderWithAlgorithm(algorithm);

            //When
            pathfinder.GetNextStep(_map, _startingPosition);

            //Then
            Assert.IsTrue(monitor.PathDetails.StepsTaken >= 1);
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldNumberOfVisitsPerNodeAfterOneStepOfPathfinding(ePathfindingAlgorithms algorithm)
        {
            //Given
            AlgorithmMonitor monitor = new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry());
            PathfinderFactory factory = new PathfinderFactory(monitor);
            Pathfinder pathfinder = factory.CreatePathfinderWithAlgorithm(algorithm);

            //When
            pathfinder.GetNextStep(_map, _startingPosition);

            //Then
            Assert.IsTrue(monitor.PathDetails.NumberOfVisitsPerNode.Count >= 1);
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldPathLengthAfterOneStepOfPathfinding(ePathfindingAlgorithms algorithm)
        {
            //Given
            AlgorithmMonitor monitor = new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry());
            PathfinderFactory factory = new PathfinderFactory(monitor);
            Pathfinder pathfinder = factory.CreatePathfinderWithAlgorithm(algorithm);

            //When
            pathfinder.GetNextStep(_map, _startingPosition);

            //Then
            Assert.AreNotEqual(0, monitor.PathDetails.PathLengthInEdgeCost);
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldLogStepsTakenAfterManyStepsOfPathfinding(ePathfindingAlgorithms algorithm)
        {
            //Given
            AlgorithmMonitor monitor = new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry());
            PathfinderFactory factory = new PathfinderFactory(monitor);
            Pathfinder pathfinder = factory.CreatePathfinderWithAlgorithm(algorithm);

            //When
            pathfinder.GetOptimalPath(_map, _startingPosition);

            //Then
            Assert.IsTrue(monitor.PathDetails.StepsTaken >= 1);
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldNumberOfVisitsPerNodeManyStepsOfPathfinding(ePathfindingAlgorithms algorithm)
        {
            //Given
            AlgorithmMonitor monitor = new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry());
            PathfinderFactory factory = new PathfinderFactory(monitor);
            Pathfinder pathfinder = factory.CreatePathfinderWithAlgorithm(algorithm);

            //When
            pathfinder.GetOptimalPath(_map, _startingPosition);

            //Then
            Assert.IsTrue(monitor.PathDetails.NumberOfVisitsPerNode.Count >= 1);
        }

        [TestCase(ePathfindingAlgorithms.Djikstra)]
        [TestCase(ePathfindingAlgorithms.BellmanFord)]
        [TestCase(ePathfindingAlgorithms.AStar)]
        [TestCase(ePathfindingAlgorithms.FloydWarshall)]
        [TestCase(ePathfindingAlgorithms.Johnson, Ignore = "Not implemented")]
        public void ShouldPathLengthAfterManyStepsOfPathfinding(ePathfindingAlgorithms algorithm)
        {
            //Given
            AlgorithmMonitor monitor = new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry());
            PathfinderFactory factory = new PathfinderFactory(monitor);
            Pathfinder pathfinder = factory.CreatePathfinderWithAlgorithm(algorithm);

            //When
            pathfinder.GetOptimalPath(_map, _startingPosition);

            //Then
            Assert.AreNotEqual(0, monitor.PathDetails.PathLengthInEdgeCost);
        }
    }
}
