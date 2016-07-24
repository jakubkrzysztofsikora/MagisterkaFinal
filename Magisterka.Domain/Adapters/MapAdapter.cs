using System.Collections.Generic;
using System.Linq;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Helpers;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.ViewModels;

namespace Magisterka.Domain.Adapters
{
    public class MapAdapter
    {
        private readonly IPathfinderFactory _pathfinderFactory;
        private Map _logicMap;
        private Pathfinder _pathfinder;

        private MapAdapter(Map logicMap, IPathfinderFactory pathfinderFactory)
        {
            _logicMap = logicMap;
            _pathfinderFactory = pathfinderFactory;
        }

        public MapView VisualMap { get; set; }

        public NodeView StartPathfinding(NodeView currentNode, ePathfindingAlgorithms algorithm)
        {
            _pathfinder = _pathfinderFactory.CreatePathfinderWithAlgorithm(algorithm);
            Position newPosition = _pathfinder.GetNextStep(_logicMap, currentNode.LogicNode.Coordinates);
            NodeView newNode = VisualMap.GetVertexByLogicNode(_logicMap.GetNodeByPosition(newPosition));

            return newNode;
        }

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
                LogicNode = node,
                Text = node.Name,
                CurrentState = eVertexState.Other
            }).ForEach(nodeView =>
            {
                VisualMap.AddVertex(nodeView);
            });
        }

        protected void ConvertLogicEdgesToVisualEdges()
        {
            IEnumerable<Edge> logicEdges = _logicMap.GetAllEdges();
            IEnumerable<EdgeAdapter> edgeAdapters = logicEdges.Select(edge => new EdgeAdapter
            {
                Edge = edge,
                FromNode = edge.NodesConnected.Key,
                ToNode = edge.NodesConnected.Value
            });
            List<EdgeAdapter> bidirectionalEdgeAdapters = new List<EdgeAdapter>(edgeAdapters);
            bidirectionalEdgeAdapters.AddRange(DuplicateAndMirrorEdgeAdapterCollection(edgeAdapters));


            IEnumerable<EdgeView> visualEdges = ConstructListOfVisualEdges(bidirectionalEdgeAdapters);

            VisualMap.AddEdgeRange(visualEdges);
        }

        private IEnumerable<EdgeView> ConstructListOfVisualEdges(IEnumerable<EdgeAdapter> edgeAdapters)
        {
            int edgeCounter = 0;
            return edgeAdapters.Select(
                    edgeAdapter =>
                        new EdgeView(edgeAdapter.Edge, VisualMap.GetVertexByLogicNode(edgeAdapter.FromNode),
                            VisualMap.GetVertexByLogicNode(edgeAdapter.ToNode))
                        {
                            ID = edgeCounter++,
                            SkipProcessing = ProcessingOptionEnum.Freeze,
                            Caption = $"{edgeAdapter.FromNode.Name} => {edgeAdapter.ToNode.Name} - Cost: {edgeAdapter.Edge.Cost}"
                        });
        }

        private IEnumerable<EdgeAdapter> DuplicateAndMirrorEdgeAdapterCollection(IEnumerable<EdgeAdapter> edgeAdapters)
        {
            return edgeAdapters.Select(edgeAdapter => edgeAdapter.GetEdgeAdapterWithMirroredEdges());
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

        public static MapAdapter CreateMapAdapterFromLogicMap(Map logicMap, IPathfinderFactory pathfinderFactory)
        {
            MapAdapter adapter = new MapAdapter(logicMap, pathfinderFactory)
            {
                VisualMap = new MapView()
            };
            adapter.ConvertLogicNodesToVisualVerticles();
            adapter.ConvertLogicEdgesToVisualEdges();
            
            return adapter;
        }
    }
}
