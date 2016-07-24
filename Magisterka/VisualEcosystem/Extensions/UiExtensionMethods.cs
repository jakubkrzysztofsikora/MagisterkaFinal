using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GraphX.Controls;
using Magisterka.Domain.ViewModels;

namespace Magisterka.VisualEcosystem.Extensions
{
    public static class UiExtensionMethods
    {
        public static NodeView GetNodeView(this VertexControl vertexControl)
        {
            return vertexControl.DataContext as NodeView;
        }

        public static EdgeView GetEdgeView(this EdgeControl edgeControl)
        {
            return edgeControl.DataContext as EdgeView;
        }

        public static VertexControl GetVertexControl(this Control item)
        {
            return (VertexControl) ((ItemsControl)item).Tag;
        }

        public static NodeView GetNodeViewFromUiElement(this Control item)
        {
            return item.GetVertexControl().GetNodeView();
        }

        public static eVertexState GetState(this VertexControl vertexControl)
        {
            return vertexControl.GetNodeView().CurrentState;
        }

        public static void SetState(this VertexControl vertexControl, eVertexState state)
        {
            vertexControl.Background = new SolidColorBrush((Color)Application.Current.Resources[state + "NodeColor" ]);
            vertexControl.GetNodeView().CurrentState = state;
        }

        public static VertexControl GetVertexControlOfNode(this VisualMap map, NodeView node)
        {
            return map.Children.OfType<VertexControl>()
                .SingleOrDefault(vertex => ((NodeView)vertex.Vertex).LogicNode == node.LogicNode);
        }

        public static EdgeControl GetEdgeControlBetweenNodes(this VisualMap map, NodeView fromNode, NodeView toNode)
        {
            if (!fromNode.LogicNode.IsNeighborWith(toNode.LogicNode))
                return null;

            return
                map.Children.OfType<EdgeControl>()
                    .FirstOrDefault(
                        edge =>
                            ((EdgeView) edge.DataContext).Source.LogicNode == fromNode.LogicNode &&
                            ((EdgeView) edge.DataContext).Target.LogicNode == toNode.LogicNode);
        }
    }
}
