using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.Controls;
using GraphX.Controls.Models;
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
        private Map _logicMap;

        public void SetAsStartingPoint(NodeView nodeView)
        {
            _logicMap = _logicMap.WithStartingPosition(nodeView.LogicNode.Coordinates);
            
            ClearVisualMapPredefinedStartingPosition();
            nodeView.Caption = DomainConstants.StartingNodeCaption;
        }

        public void SetAsTargetPoint(NodeView nodeView)
        {
            _logicMap = _logicMap.WithTargetPosition(nodeView.LogicNode.Coordinates);

            ClearVisualMapPredefinedTargetPosition();
            nodeView.Caption = DomainConstants.TargetNodeCaption;
        }

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
                AddEdgesToVisualMap(edges, eEdgeDirection.In);
                AddEdgesToVisualMap(edges, eEdgeDirection.Out, edges.Count());
            });
        }

        private MapAdapter(Map logicMap)
        {
            _logicMap = logicMap;
        }

        private void ClearVisualMapPredefinedStartingPosition()
        {
            Node startingNode = _logicMap.SingleOrDefault(node => node.IsStartingNode);

            NodeView startingVisualNode = VisualMap.GetVertexByLogicNode(startingNode);
            startingVisualNode.Caption = string.Empty;
        }

        private void ClearVisualMapPredefinedTargetPosition()
        {
            Node targetNode = _logicMap.SingleOrDefault(node => node.IsTargetNode);

            NodeView targetVisualNode = VisualMap.GetVertexByLogicNode(targetNode);
            targetVisualNode.Caption = string.Empty;
        }

        private void AddEdgesToVisualMap(IEnumerable<Edge> edges, eEdgeDirection direction, long edgeCounter = 0)
        {
            VisualMap.AddEdgeRange(
                    edges.Select(edge => new EdgeView(edge, VisualMap.GetVertexByLogicNode(direction == eEdgeDirection.In ?  edge.NodesConnected.Key : edge.NodesConnected.Value),
                        VisualMap.GetVertexByLogicNode(direction == eEdgeDirection.In ? edge.NodesConnected.Value : edge.NodesConnected.Key))
                    {
                        ID = edgeCounter++,
                        SkipProcessing = ProcessingOptionEnum.Freeze,
                        Caption = $"Node {VisualMap.GetVertexByLogicNode(edge.NodesConnected.Key).ID} => Node {VisualMap.GetVertexByLogicNode(edge.NodesConnected.Value).ID} - Cost: {edge.Cost}"
                    }));
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
