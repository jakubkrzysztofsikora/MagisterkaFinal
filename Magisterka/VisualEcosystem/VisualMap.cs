using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using GraphX.Controls;
using GraphX.Controls.Models;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using GraphX.PCL.Logic.Models;
using Magisterka.Domain.ViewModels;
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
                vertexControl.Background = Brushes.Crimson;
            }
        }
    }
}
