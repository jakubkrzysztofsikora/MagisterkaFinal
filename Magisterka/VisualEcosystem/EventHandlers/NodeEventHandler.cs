using System.Windows;
using System.Windows.Controls;
using GraphX.Controls;
using GraphX.Controls.Models;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.EventHandlers
{
    public class NodeEventHandler
    {
        public static string NameOfNodeContextMenu { get; set; }
        public static string NameOfSetAsBlocked { get; set; }
        public static string NameOfSetAsUnblocked { get; set; }

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
            var tooltip = (ToolTip) e.VertexControl.ToolTip;
            if (tooltip != null)
            {
                tooltip.IsOpen = false;
                e.VertexControl.ToolTip = null;
            }
        }

        public static void OnNodeRightClick(object sender, VertexSelectedEventArgs e)
        {
            ContextMenu contextMenu = Application.Current.MainWindow.FindResource(NameOfNodeContextMenu) as ContextMenu;
            contextMenu.PlacementTarget = sender as Button;
            contextMenu.IsOpen = true;
            AddVertexControlsToMenuTags(contextMenu, e.VertexControl);

            MenuItem setAsBlockedItem = LogicalTreeHelper.FindLogicalNode(contextMenu, NameOfSetAsBlocked) as MenuItem;
            setAsBlockedItem.Visibility = e.VertexControl.GetNodeView().LogicNode.IsBlocked
                ? Visibility.Collapsed
                : Visibility.Visible;
            
            MenuItem setAsUnblockedItem = LogicalTreeHelper.FindLogicalNode(contextMenu, NameOfSetAsUnblocked) as MenuItem;
            setAsUnblockedItem.Visibility = e.VertexControl.GetNodeView().LogicNode.IsBlocked
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private static void SetMenuPositionDependantOnProperty(ContextMenu menu, bool booleanProperty, string nameOfMenuPosition)
        {
            MenuItem menuPosition = LogicalTreeHelper.FindLogicalNode(menu, nameOfMenuPosition) as MenuItem;
            menuPosition.Visibility = booleanProperty
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private static void AddVertexControlsToMenuTags(ContextMenu menu, VertexControl vertex)
        {
            foreach (Control menuPosition in menu.Items)
            {
                menuPosition.Tag = vertex;
            }
        }
    }
}
