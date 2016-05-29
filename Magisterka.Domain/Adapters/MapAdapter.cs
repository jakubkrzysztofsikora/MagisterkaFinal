using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Helpers;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.ViewModels;

namespace Magisterka.Domain.Adapters
{
    public class MapAdapter
    {
        public MapView VisualMap { get; set; }
        private readonly Map _logicMap;

        protected void ConvertLogicNodesToVisualVerticles()
        {
            long nodeCounter = 0;
            _logicMap.Select(node => new NodeView
            {
                ID = nodeCounter++,
                LogicNode = node
            }).ForEach(nodeView =>
            {
                VisualMap.AddVertex(nodeView);
            });
        }

        protected void ConvertLogicEdgesToVisualEdges()
        {
            long edgeCounter = 0;
            VisualMap.Vertices.Select(nodeView => nodeView.LogicNode).ForEach(node =>
            {
                IEnumerable<Edge> edges = node.Neighbors.Select(neighborToEdge => neighborToEdge.Value);
                VisualMap.AddEdgeRange(
                    edges.Select(edge => new EdgeView(edge, VisualMap.GetVertexByLogicNode(edge.NodesConnected.Key),
                        VisualMap.GetVertexByLogicNode(edge.NodesConnected.Value))
                    {
                        ID = edgeCounter++,
                        SkipProcessing = ProcessingOptionEnum.Freeze,
                        Caption = $"Node {VisualMap.GetVertexByLogicNode(edge.NodesConnected.Key).ID} => Node {VisualMap.GetVertexByLogicNode(edge.NodesConnected.Value).ID} - Cost: {edge.Cost}"
                    }));
                VisualMap.AddEdgeRange(
                     edges.Select(edge => new EdgeView(edge, VisualMap.GetVertexByLogicNode(edge.NodesConnected.Value),
                         VisualMap.GetVertexByLogicNode(edge.NodesConnected.Key))
                     {
                         ID = edgeCounter++,
                         SkipProcessing = ProcessingOptionEnum.Freeze,
                         Caption = $"Node {VisualMap.GetVertexByLogicNode(edge.NodesConnected.Key).ID} => Node {VisualMap.GetVertexByLogicNode(edge.NodesConnected.Value).ID} - Cost: {edge.Cost}"
                     }));
            });
        }

        private MapAdapter(Map logicMap)
        {
            _logicMap = logicMap;
        }

        public static MapAdapter CreateMapAdapterFromLogicMap(Map logicMap)
        {
            MapAdapter adapter = new MapAdapter(logicMap);
            adapter.VisualMap = new MapView();
            adapter.ConvertLogicNodesToVisualVerticles();
            adapter.ConvertLogicEdgesToVisualEdges();
            
            return adapter;
        }
    }
}
