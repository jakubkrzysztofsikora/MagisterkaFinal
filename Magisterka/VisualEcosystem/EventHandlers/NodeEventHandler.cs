using System.Windows;
using System.Windows.Controls;
using GraphX.Controls.Models;
using Magisterka.Domain.ViewModels;

namespace Magisterka.VisualEcosystem.EventHandlers
{
    public class NodeEventHandler
    {
        public static void OnNodeHoverIn(object sender, VertexSelectedEventArgs e)
        {
            var tooltip = new ToolTip();
            var nodeView = e.VertexControl.Vertex as NodeView;
            tooltip.Content = $"{nodeView?.LogicNode.Name}";
            tooltip.IsOpen = true;
            e.VertexControl.ToolTip = tooltip;
        }

        public static void OnNodeHoverOut(object sender, VertexSelectedEventArgs e)
        {
            ((ToolTip)e.VertexControl.ToolTip).IsOpen = false;
            e.VertexControl.ToolTip = null;
        }

        public static void OnNodeRightClick(object sender, VertexSelectedEventArgs e)
        {
            ContextMenu contextMenu = Application.Current.MainWindow.FindResource("NodeContextMenu") as ContextMenu;
            contextMenu.PlacementTarget = sender as Button;
            contextMenu.IsOpen = true;
            foreach (Control menuPosition in contextMenu.Items)
            {
                menuPosition.Tag = e.VertexControl;
            }
        }
    }
}
