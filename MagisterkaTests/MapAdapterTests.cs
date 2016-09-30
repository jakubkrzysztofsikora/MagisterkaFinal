using System.Collections.Generic;
using System.Linq;
using GraphX.PCL.Logic.Algorithms;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.AppSettings;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using Magisterka.Domain.Monitoring.Performance;
using Magisterka.Domain.Monitoring.Quality;
using Magisterka.Domain.Utilities;
using Magisterka.Domain.ViewModels;
using NUnit.Framework;

namespace MagisterkaTests
{
    [TestFixture]
    public class MapAdapterTests
    {
        [Test]
        public void ShouldAddAnEdgeToBothMaps()
        {
            //Given
            MapFactory logicMapFactory = new MapFactory(new DefaultRandomGenerator(), new AppSettings());
            Map logicMap = logicMapFactory.GenerateDefaultMap();
            MapAdapter mapAdapter = MapAdapter.CreateMapAdapterFromLogicMap(logicMap, new PathfinderFactory(new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry())), logicMapFactory);
            NodeView node1 = new NodeView
            {
                LogicNode = new Node("test1")
            };
            NodeView node2 = new NodeView
            {
                LogicNode = new Node("test2")
            };
            mapAdapter.AddNode(node1);
            mapAdapter.AddNode(node2);
            EdgeView edge = new EdgeView(new Edge
            {
                Cost = 2,
                NodesConnected = new KeyValuePair<Node, Node>(node1.LogicNode, node2.LogicNode)
            }, node1, node2);

            //When
            mapAdapter.AddEdge(edge);

            //Then
            Assert.IsTrue(logicMap.GetAllEdges().Any(e => e == edge.LogicEdge) && mapAdapter.VisualMap.ContainsEdge(edge));
        }

        [Test]
        public void ShouldAddANodeToBothMaps()
        {
            //Given
            MapFactory logicMapFactory = new MapFactory(new DefaultRandomGenerator(), new AppSettings());
            Map logicMap = logicMapFactory.GenerateDefaultMap();
            MapAdapter mapAdapter = MapAdapter.CreateMapAdapterFromLogicMap(logicMap, new PathfinderFactory(new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry())), logicMapFactory);
            NodeView node = new NodeView
            {
                LogicNode = new Node("test")
            };

            //When
            mapAdapter.AddNode(node);

            //Then
            Assert.IsTrue(logicMap.Contains(node.LogicNode) && mapAdapter.VisualMap.ContainsVertex(node));
        }

        [Test]
        public void ShouldDeleteAnEdgeOnBothMaps()
        {
            //Given
            MapFactory logicMapFactory = new MapFactory(new DefaultRandomGenerator(), new AppSettings());
            Map logicMap = logicMapFactory.GenerateDefaultMap();
            MapAdapter mapAdapter = MapAdapter.CreateMapAdapterFromLogicMap(logicMap, new PathfinderFactory(new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry())), logicMapFactory);
            EdgeView edgeToDelete = mapAdapter.VisualMap.GetAllEdges(mapAdapter.VisualMap.Vertices.First()).First();

            //When
            mapAdapter.DeleteEdge(edgeToDelete);

            //Then
            bool edgeExistsOnVisualMap = mapAdapter.VisualMap.ContainsEdge(edgeToDelete);
            bool edgexistsOnLogicMap = logicMap.GetAllEdges().Contains(edgeToDelete.LogicEdge);
            Assert.IsTrue(!edgeExistsOnVisualMap && !edgexistsOnLogicMap);
        }

        [Test]
        public void ShouldDeleteANodeOnBothMaps()
        {
            //Given
            MapFactory logicMapFactory = new MapFactory(new DefaultRandomGenerator(), new AppSettings());
            Map logicMap = logicMapFactory.GenerateDefaultMap();
            MapAdapter mapAdapter = MapAdapter.CreateMapAdapterFromLogicMap(logicMap, new PathfinderFactory(new AlgorithmMonitor(new PerformanceMonitor(), new AlgorithmQualityRegistry())), logicMapFactory);
            NodeView nodeToDelete = mapAdapter.VisualMap.GetVertexByLogicNode(logicMap.First());

            //When
            mapAdapter.DeleteNode(nodeToDelete);

            //Then
            bool nodeExistsOnVisualMap = mapAdapter.VisualMap.ContainsVertex(nodeToDelete);
            bool nodeExistsOnLogicMap = logicMap.Contains(nodeToDelete.LogicNode);
            Assert.IsTrue(!nodeExistsOnVisualMap && !nodeExistsOnLogicMap);
        }
    }
}
