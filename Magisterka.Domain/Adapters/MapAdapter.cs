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
        public MapView VisualMap { get; set; }

        private readonly IPathfinderFactory _pathfinderFactory;
        private Map _logicMap;
        private Pathfinder _pathfinder;
        private bool _graphChanged;

        private MapAdapter(Map logicMap, IPathfinderFactory pathfinderFactory)
        {
            _logicMap = logicMap;
            _pathfinderFactory = pathfinderFactory;
            _graphChanged = true;
        }

        public bool CanStartPathfinding()
        {
            return _logicMap.Any(node => node.IsStartingNode) && _logicMap.Any(node => node.IsTargetNode);
        }

        public NodeView StartPathfindingByStep(NodeView currentNode, ePathfindingAlgorithms algorithm)
        {
            _pathfinder = _pathfinderFactory.CreatePathfinderWithAlgorithm(algorithm);
            Position newPosition = _pathfinder.GetNextStep(_logicMap, currentNode.LogicNode.Coordinates);
            NodeView newNode = VisualMap.GetVertexByLogicNode(_logicMap.GetNodeByPosition(newPosition));
            _graphChanged = false;

            return newNode;
        }

        public IEnumerable<NodeView> StartPathfindingAllRoute(NodeView currentNode, ePathfindingAlgorithms algorithm)
        {
            _pathfinder = _pathfinderFactory.CreatePathfinderWithAlgorithm(algorithm);
            var logicPath = _pathfinder.GetOptimalPath(_logicMap, currentNode.LogicNode.Coordinates, _graphChanged);
            _graphChanged = false;

            return logicPath.Select(node => VisualMap.GetVertexByLogicNode(node));
        }

        public void ClearGraph()
        {
            ClearVisualMapPredefinedStartingPosition();
            ClearVisualMapPredefinedTargetPosition();
            _logicMap.WithNoDefinedPositions();
        }

        public void DeleteGraphData()
        {
            _logicMap.ForEach(node =>
            {
                _logicMap.Remove(node);
            });
            
            VisualMap = new MapView();
        }

        public void SetAsStartingPoint(NodeView nodeView)
        {
            _logicMap = _logicMap.WithStartingPosition(nodeView.LogicNode.Coordinates);
            
            ClearVisualMapPredefinedStartingPosition();
            nodeView.Caption = DomainConstants.StartingNodeCaption;
            _graphChanged = true;
        }

        public void SetAsTargetPoint(NodeView nodeView)
        {
            _logicMap = _logicMap.WithTargetPosition(nodeView.LogicNode.Coordinates);

            ClearVisualMapPredefinedTargetPosition();
            nodeView.Caption = DomainConstants.TargetNodeCaption;
            _graphChanged = true;
        }

        public void DeleteNode(NodeView node)
        {
            _logicMap.Delete(node.LogicNode);
            VisualMap.RemoveVertex(node);
            _graphChanged = true;
        }

        public void DeleteEdge(EdgeView edge)
        {
            _logicMap.Delete(edge.LogicEdge);
            VisualMap.RemoveEdge(edge);
            _graphChanged = true;
        }

        public void AddNode(NodeView node)
        {
            VisualMap.AddVertex(node);
            _logicMap.Add(node.LogicNode);
            _graphChanged = true;
        }

        public void AddEdge(EdgeView edge)
        {
            VisualMap.AddEdge(edge);
            _logicMap.AddEdge(edge.LogicEdge);
            _graphChanged = true;
        }

        public void ChangeCost(EdgeView edge, int answer)
        {
            VisualMap.ChangeEdgeCost(edge, answer);
            edge.LogicEdge.Cost = answer;
            _graphChanged = true;
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
                            SkipProcessing = ProcessingOptionEnum.Freeze
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
    }
}
