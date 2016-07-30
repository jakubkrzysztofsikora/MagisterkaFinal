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

namespace Magisterka.VisualEcosystem
{
    public class VisualMap : GraphArea<NodeView, EdgeView, BidirectionalGraph<NodeView, EdgeView>>
    {
        public event VertexSelectedEventHandler VertexRightClick;
        public event EdgeSelectedEventHandler EdgeRightClick;

        public void InitilizeVisuals()
        {
            GenerateGraph();
            SetEdgesDashStyle(EdgeDashStyle.Dot);
            SetVerticesMathShape(VertexShape.Ellipse);
            SetVerticesDrag(true, true);
            ShowAllVerticesLabels(false);
            ShowAllEdgesLabels(false);
            ShowAllEdgesArrows(false);
            
            SetStateColors();
            MarkBlockedNodes();
        }

        public void InitilizeLogicCore(MapView visualMap)
        {
            var logicCore = new GXLogicCore<NodeView, EdgeView, BidirectionalGraph<NodeView, EdgeView>>
            {
                Graph = visualMap,
                DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.Bundling,
                DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA,
                DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.CompoundFDP,
                EnableParallelEdges = false,
                EdgeCurvingEnabled = true,
                AsyncAlgorithmCompute = true,
                DefaultOverlapRemovalAlgorithmParams =
                {
                    HorizontalGap = 100,
                    VerticalGap = 100
                }
            };


            LogicCore = logicCore;
        }

        public void InitializeEventHandlers()
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
            };
        }

        public void AddCustomChildIfNotExists(UIElement element)
        {
            if (!Children.Contains(element))
                AddCustomChildControl(element);
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

        public void ClearGraph()
        {
            RemoveStartLabel();
            RemoveTargetLabel();
            RemoveStateFromPreviousNode(eVertexState.Start);
            RemoveStateFromPreviousNode(eVertexState.Target);
            RemoveStateFromPreviousNode(eVertexState.Current);
            RemoveStateFromPreviousNode(eVertexState.Visited);
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
                vertexControl.Background = new SolidColorBrush((Color)Application.Current.Resources["BlockedNodeColor"]);
            }
        }
    }
}
