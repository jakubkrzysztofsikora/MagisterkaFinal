using System;
using System.Linq;
using GraphX.PCL.Logic.Algorithms;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using Magisterka.Domain.Monitoring.Performance;
using Magisterka.Domain.Monitoring.Quality;
using Magisterka.Domain.ViewModels;
using NUnit.Framework;

namespace MagisterkaTests
{
    [TestFixture]
    public class MapAdapterTests
    {
        [Test]
        public void ShouldDeleteANodeOnBothMaps()
        {
            //Given
            MapFactory logicMapFactory = new MapFactory(new Random());
            Map logicMap = logicMapFactory.GenerateDefaultMap();
            MapAdapter mapAdapter = MapAdapter.CreateMapAdapterFromLogicMap(logicMap, new PathfinderFactory(new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry())));
            NodeView nodeToDelete = mapAdapter.VisualMap.GetVertexByLogicNode(logicMap.First());

            //When
            mapAdapter.Delete(nodeToDelete);

            //Then
            bool nodeExistsOnVisualMap = mapAdapter.VisualMap.ContainsVertex(nodeToDelete);
            bool nodeExistsOnLogicMap = logicMap.Contains(nodeToDelete.LogicNode);
            Assert.IsTrue(!nodeExistsOnVisualMap && !nodeExistsOnLogicMap);
        }

        [Test]
        public void ShouldDeleteAnEdgeOnBothMaps()
        {
            MapFactory logicMapFactory = new MapFactory(new Random());
            Map logicMap = logicMapFactory.GenerateDefaultMap();
            MapAdapter mapAdapter = MapAdapter.CreateMapAdapterFromLogicMap(logicMap, new PathfinderFactory(new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry())));
            EdgeView edgeToDelete = mapAdapter.VisualMap.GetAllEdges(mapAdapter.VisualMap.Vertices.First()).First();

            //When
            mapAdapter.Delete(edgeToDelete);

            //Then
            bool edgeExistsOnVisualMap = mapAdapter.VisualMap.ContainsEdge(edgeToDelete);
            bool edgexistsOnLogicMap = logicMap.GetAllEdges().Contains(edgeToDelete.LogicEdge);
            Assert.IsTrue(!edgeExistsOnVisualMap && !edgexistsOnLogicMap);
        }
    }
}
