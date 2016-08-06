using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Castle.Core.Internal;
using GraphX.Controls;
using GraphX.Controls.Models;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Models;
using Magisterka.Domain;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.Animation.AnimationCommands;
using Magisterka.VisualEcosystem.Extensions;
using QuickGraph;
using Point = GraphX.Measure.Point;

namespace Magisterka.VisualEcosystem
{
    public class VisualMap : GraphArea<NodeView, EdgeView, BidirectionalGraph<NodeView, EdgeView>>
    {
        private IAnimationCommand _pathfindingAnimation;
        public bool VerticlesDragging { get; set; }
        public bool ShowVerticlesLabels { get; set; }
        public bool ShowEdgeLabels { get; set; }
        public bool ShowEdgeArrows { get; set; }
        public event VertexSelectedEventHandler VertexRightClick;
        public event EdgeSelectedEventHandler EdgeRightClick;

        public void InitilizeVisuals()
        {
            GenerateGraph();
            SetEdgesDashStyle(EdgeDashStyle.Solid);
            SetVerticesMathShape(VertexShape.Ellipse);
            SetVerticesDrag(VerticlesDragging, true);
            ShowAllVerticesLabels(ShowVerticlesLabels);
            ShowAllEdgesLabels(ShowEdgeLabels);
            
            SetStateColors();
            MarkBlockedNodes();
        }

        public void InitilizeLogicCore(MapView visualMap)
        {
            var logicCore = new GXLogicCore<NodeView, EdgeView, BidirectionalGraph<NodeView, EdgeView>>
            {
                Graph = visualMap,
                DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER,
                DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA,
                DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.CompoundFDP,
                EnableParallelEdges = false,
                EdgeCurvingEnabled = true,
                AsyncAlgorithmCompute = true,
                DefaultOverlapRemovalAlgorithmParams =
                {
                    HorizontalGap = 80,
                    VerticalGap = 80
                }
            };


            LogicCore = logicCore;
        }

        public void InitializeGraphElementsEventHandlers()
        {
            GenerateGraphFinished += delegate
            {
                AddRightClickEventHandlerToElements<VertexControl, VertexSelectedEventHandler, VertexSelectedEventArgs>(
                VertexRightClick,
                (vertexControl, mouseEventArgs, modifierKey) =>
                    new VertexSelectedEventArgs(vertexControl, mouseEventArgs, modifierKey));

                AddRightClickEventHandlerToElements<EdgeControl, EdgeSelectedEventHandler, EdgeSelectedEventArgs>(
                EdgeRightClick,
                (edgeControl, mouseArgs, modifierKey) =>
                    new EdgeSelectedEventArgs(edgeControl, mouseArgs, modifierKey));

                ShowAllEdgesArrows(ShowEdgeArrows);
            };
        }

        public void AddVertex(NodeView vertex)
        {
            var newVertexControl = new VertexControl(vertex);
            AddVertex(vertex, newVertexControl);
            newVertexControl.SetPosition(0, 0);
            newVertexControl.PreviewMouseRightButtonDown += (sender, args) => VertexRightClick?.Invoke(sender, new VertexSelectedEventArgs(newVertexControl, args, ModifierKeys.None));
            RefreshGraph();
        }

        public void AddEdge(EdgeView edge, VertexControl fromVertexControl, VertexControl toVertexControl)
        {
            var newEdgeControl = new EdgeControl(fromVertexControl, toVertexControl, edge)
            {
                ShowArrows = ShowEdgeArrows,
                ShowLabel = ShowEdgeLabels
            };

            newEdgeControl.PreviewMouseRightButtonDown +=
                (sender, args) =>
                    EdgeRightClick?.Invoke(sender, new EdgeSelectedEventArgs(newEdgeControl, args, ModifierKeys.None));
            AddEdge(edge, newEdgeControl);
            newEdgeControl.ShowArrows = false;
        }

        public void SetVerticesDrag(bool newValue)
        {
            SetVerticesDrag(newValue, false);
            VerticlesDragging = newValue;
        }

        public new void ShowAllEdgesLabels(bool newValue)
        {
            ShowAllEdgesLabels(isEnabled: newValue);
            ShowEdgeLabels = newValue;
        }

        public new void ShowAllEdgesArrows(bool newValue)
        {
            ShowAllEdgesArrows(isEnabled: newValue);
            ShowEdgeArrows = newValue;
        }

        public void RefreshGraph()
        {
            Children.OfType<VertexControl>().Where(vertex => vertex.GetState() == eVertexState.Other).ForEach(vertex => vertex.SetState(eVertexState.Other));
            MarkBlockedNodes();
        }

        public void AddCustomChildIfNotExists(UIElement element, Point? location = null)
        {
            if (!Children.Contains(element))
            {
                AddCustomChildControl(element);

                if (location == null)
                    return;

                SetLeft(element, location.Value.X);
                SetTop(element, location.Value.Y);
            }
        }

        public void SetCurrentNode(VertexControl vertex)
        {
            var oldVertex = Children.OfType<VertexControl>()
                .SingleOrDefault(vertexControl => vertexControl.GetState() == eVertexState.Current);

            oldVertex?.SetState(eVertexState.Visited);
            vertex.SetState(eVertexState.Current);
        }

        public void SetStartingNode(VertexControl vertex)
        {
            RemoveStartLabel();
            CreateLabelForNode(vertex);
            RemoveStateFromPreviousNode(eVertexState.Start);
            vertex.SetState(eVertexState.Start);
        }

        public void SetTargetNode(VertexControl vertex)
        {
            RemoveTargetLabel();
            CreateLabelForNode(vertex);
            RemoveStateFromPreviousNode(eVertexState.Target);
            vertex.SetState(eVertexState.Target);
        }

        public void MarkBlockedNode(VertexControl vertex)
        {
            vertex.Background = new SolidColorBrush((Color)Application.Current.Resources["BlockedNodeColor"]);
        }

        public void MarkUnblockedNode(VertexControl vertex)
        {
            vertex.SetState(eVertexState.Other);
        }

        public void ClearGraph()
        {
            RemoveStartLabel();
            RemoveTargetLabel();
            RemoveStateFromPreviousNode(eVertexState.Start);
            RemoveStateFromPreviousNode(eVertexState.Target);
            RemoveStateFromPreviousNode(eVertexState.Current);
            RemoveStateFromPreviousNode(eVertexState.Visited);
            _pathfindingAnimation.StopAnimation();
        }

        public VertexControl GetCurrentVertex()
        {
            return Children.OfType<VertexControl>()
                .SingleOrDefault(vertex => vertex.GetState() == eVertexState.Current) ?? Children.OfType<VertexControl>()
                .SingleOrDefault(vertex => vertex.GetState() == eVertexState.Start);
        }

        public void CreateLabelForNode(VertexControl vertexControl)
        {
            DefaultVertexlabelFactory factory = new DefaultVertexlabelFactory();
            AttachableVertexLabelControl label = factory.CreateLabel(vertexControl);
            label.Content = label.Name = vertexControl.GetNodeView().Caption;
            label.IsEnabled = true;
            label.Background = new SolidColorBrush((Color)Application.Current.Resources["LabelBackgroundColor"]);
            label.LabelPositionMode = VertexLabelPositionMode.Sides;
            label.LabelPositionSide = VertexLabelPositionSide.Bottom;
            vertexControl.AttachLabel(label);
            vertexControl.ShowLabel = true;
            AddCustomChildIfNotExists(label);
        }

        public void GoToVertex(VertexControl nextVertex, IAnimationCommand animation)
        {
            _pathfindingAnimation = animation;
            animation.AnimationEnded += (o, args) => SetCurrentNode(nextVertex);
            animation.BeginAnimation();
        }

        public void GoToVertex(VertexControl nextVertex)
        {
            SetCurrentNode(nextVertex);
        }

        private void SetStateColors()
        {
            Children.OfType<VertexControl>().ForEach(vertex => vertex.SetState(vertex.GetState()));
        }

        private void RemoveStartLabel()
        {
            var labelToDelete = Children.OfType<VertexLabelControl>().SingleOrDefault(label => label.Name == DomainConstants.StartingNodeCaption);
            RemoveCustomChildControl(labelToDelete);
        }

        private void RemoveStateFromPreviousNode(eVertexState state)
        {
            Children.OfType<VertexControl>()
                .Where(vertex => vertex.GetState() == state).ForEach(vertex => vertex.SetState(eVertexState.Other));
        }

        private void RemoveTargetLabel()
        {
            var labelToDelete = Children.OfType<VertexLabelControl>().SingleOrDefault(label => label.Name == DomainConstants.TargetNodeCaption);
            RemoveCustomChildControl(labelToDelete);
        }

        private void AddRightClickEventHandlerToElements<TElement, TEventHandler, TEventArgs>(TEventHandler eventHandler, Func<TElement, MouseButtonEventArgs, ModifierKeys, TEventArgs> constructor)
            where TElement : Control
            where TEventArgs : EventArgs
            where TEventHandler : class 
        {
            bool isVertexEventHandler = eventHandler is VertexSelectedEventHandler;
            bool isEdgeEventHandler = eventHandler is EdgeSelectedEventHandler;
            if (!isVertexEventHandler && !isEdgeEventHandler)
                return;

            foreach (var control in Children.OfType<TElement>())
            {
                control.PreviewMouseRightButtonDown += delegate
                {
                    if (isVertexEventHandler)
                        (eventHandler as VertexSelectedEventHandler)?.Invoke(control, constructor(control, null, ModifierKeys.None) as VertexSelectedEventArgs);
                    else if (isEdgeEventHandler)
                        (eventHandler as EdgeSelectedEventHandler)?.Invoke(control, constructor(control, null, ModifierKeys.None) as EdgeSelectedEventArgs);
                };
            }
        }

        private void MarkBlockedNodes()
        {
            foreach (
                var vertexControl in Children.OfType<VertexControl>().Where(vertex => ((NodeView)vertex.Vertex).LogicNode.IsBlocked))
            {
                MarkBlockedNode(vertexControl);
            }
        }
    }
}
