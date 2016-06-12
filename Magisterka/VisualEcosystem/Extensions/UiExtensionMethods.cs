using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
            vertexControl.GetNodeView().CurrentState = state;
        }
    }
}
