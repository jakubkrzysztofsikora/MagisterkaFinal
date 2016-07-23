using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphX.Controls;
using GraphX.Controls.Models;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
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
            SetEdgesDashStyle(EdgeDashStyle.Solid);
            SetVerticesMathShape(VertexShape.Circle);
            SetVerticesDrag(true, true);
            ShowAllVerticesLabels(true);
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
                DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER,
                DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA,
                DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.KK,
                AsyncAlgorithmCompute = true
            };

            logicCore.DefaultLayoutAlgorithmParams = logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.KK);
            ((KKLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).MaxIterations = 100;

            logicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            logicCore.DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 50;
            logicCore.DefaultOverlapRemovalAlgorithmParams.VerticalGap = 50;
            logicCore.AsyncAlgorithmCompute = false;

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

            oldVertex?.SetState(eVertexState.Other);
            vertex.SetState(eVertexState.Current);
        }

        public void SetStartingNode(VertexControl vertex)
        {
            RemoveStartLabel();
            CreateLabelForNode(vertex);
            SetCurrentNode(vertex);
        }

        public void SetTargetNode(VertexControl vertex)
        {
            RemoveTargetLabel();
            CreateLabelForNode(vertex);
        }
        
        public VertexControl GetCurrentVertex()
        {
            return Children.OfType<VertexControl>()
                .SingleOrDefault(vertex => vertex.GetState() == eVertexState.Current);
        }

        public void CreateLabelForNode(VertexControl vertexControl)
        {
            DefaultVertexlabelFactory factory = new DefaultVertexlabelFactory();
            VertexLabelControl label = factory.CreateLabel(vertexControl);
            label.Content = label.Name = vertexControl.GetNodeView().Caption;
            label.IsEnabled = true;
            label.Background = new SolidColorBrush((Color)Application.Current.Resources["LabelBackgroundColor"]);
            AddCustomChildControl(label);
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
