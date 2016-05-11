using System;
using System.Linq;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using NUnit.Framework;

namespace MagisterkaTests
{
    [TestFixture]
    public class MapTests
    {
        [Test]
        public void ShouldGenerateNewProperMap()
        {
            //Given
            MapFactory factory = new MapFactory(new Random());

            //When
            Map result = factory.GenerateDefaultMap();

            //Then
            Assert.AreEqual(20, result.Count);
            Assert.False(result.Any(node => node.Neighbors.Count == 0));
            Assert.True(result.All(node => node.Neighbors.All(neighbor => neighbor.Key.IsNeighborWith(node))));
        }

        [Test]
        public void ShouldGetNodeByItsPosition()
        {
            //Given
            MapFactory factory = new MapFactory(new Random());
            Map map = factory.GenerateDefaultMap();
            Node expectedResult = map.First();
            Position positionOfTheNodeToFind = expectedResult.Coordinates;

            //When
            Node result = map.GetNodeByPosition(positionOfTheNodeToFind);

            //Then
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ShouldGenerateMapWithStartingPosition()
        {
            //Given
            MapFactory factory = new MapFactory(new Random());
            Map map = factory.GenerateDefaultMap();
            Node startingNode = map.First();

            //When
            map = map.WithStartingPosition(startingNode.Coordinates);

            //Then
            Assert.AreEqual(startingNode, map.Single(node => node.IsStartingNode));
        }

        [Test]
        public void ShouldGenerateMapWithTargetPosition()
        {
            //Given
            MapFactory factory = new MapFactory(new Random());
            Map map = factory.GenerateDefaultMap();
            Node targetNode = map.ElementAt(new Random().Next(0, map.Count - 1));

            //When
            map = map.WithTargetPosition(targetNode.Coordinates);

            //Then
            Assert.AreEqual(targetNode, map.Single(node => node.IsTargetNode));
        }

        [Test]
        public void ShouldClearAllDefinedPositions()
        {
            //Given
            MapFactory factory = new MapFactory(new Random());
            Map map = factory.GenerateDefaultMap();
            Node startingNode = map.First();
            Node targetNode = map.ElementAt(new Random().Next(0, map.Count - 1));
            map = map.WithStartingPosition(startingNode.Coordinates).WithTargetPosition(targetNode.Coordinates);

            //When
            map = map.WithNoDefinedPositions();

            //Then
            Assert.True(!map.Any(node => node.IsStartingNode || node.IsTargetNode));
        }

        [Test]
        public void ShouldCalculateHeurisitcScoreAsNumberOfNodesBetween()
        {
            //Given
            MapFactory factory = new MapFactory(new Random());
            Map map = factory.GenerateDefaultMap();
            int heuristicScore = 2;
            Node firstNode = map.First();
            Node targetNode = map.GetNodeByPosition(1, 1);

            //When
            long result = map.GetHeuristicScoreBetweenNodes(firstNode, targetNode);

            //Then
            Assert.AreEqual(heuristicScore, result);
        }

        [Test]
        public void ShouldGiveUniquePositionsToEachNode()
        {
            //Given
            MapFactory factory = new MapFactory(new Random());
            
            //When
            Map defaultMap = factory.GenerateDefaultMap();

            //Then
            Assert.True(defaultMap.All(node => node.IsOnTheGrid()));
            Assert.False(defaultMap.GroupBy(node => new { node.Coordinates.X, node.Coordinates.Y }).Any(group => group.Count() > 1));
        }

        private Node GetNodeXNodesAway(Node node, int x)
        {
            Node result = node;

            for (int i = 0; i < x; i++)
            {
                result = result?.Neighbors.Select(nodeToCost => nodeToCost.Key).FirstOrDefault();
            }

            return result;
        }
    }
}
