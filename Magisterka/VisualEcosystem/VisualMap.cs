﻿using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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

        public void InitilizeVisuals()
        {
            GenerateGraph(true);
            SetEdgesDashStyle(EdgeDashStyle.Dot);
            SetVerticesMathShape(VertexShape.Ellipse);
            SetVerticesDrag(true, true);
            ShowAllVerticesLabels(false);
            ShowAllEdgesLabels(false);
            ShowAllEdgesArrows(false);
            AddRightClickEventHandlerToVerticles();

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
                EdgeCurvingEnabled = false,
                AsyncAlgorithmCompute = true,
                DefaultOverlapRemovalAlgorithmParams =
                {
                    HorizontalGap = 100,
                    VerticalGap = 100
                }
            };


            LogicCore = logicCore;
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

        private void RemoveStartLabel()
        {
            var labelToDelete = Children.OfType<VertexLabelControl>().SingleOrDefault(label => label.Name == DomainConstants.StartingNodeCaption);
            RemoveCustomChildControl(labelToDelete);
        }

        private void RemoveStateFromPreviousNode(eVertexState state)
        {
            Children.OfType<VertexControl>()
                .SingleOrDefault(vertex => vertex.GetState() == state)?.SetState(eVertexState.Other);
        }

        private void RemoveTargetLabel()
        {
            var labelToDelete = Children.OfType<VertexLabelControl>().SingleOrDefault(label => label.Name == DomainConstants.TargetNodeCaption);
            RemoveCustomChildControl(labelToDelete);
        }

        private void AddRightClickEventHandlerToVerticles()
        {
            foreach (var vertexControl in Children.OfType<VertexControl>())
            {
                vertexControl.PreviewMouseRightButtonDown += delegate
                {
                    VertexRightClick?.Invoke(vertexControl,
                        new VertexSelectedEventArgs(vertexControl, null, ModifierKeys.None));
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
