using System.Windows;
using System.Windows.Controls;
using GraphX.Controls.Models;
using Magisterka.Domain.ViewModels;

namespace Magisterka.VisualEcosystem.EventHandlers
{
    public class EdgeEventHandler
    {
        public static void OnEdgeHoverIn(object sender, EdgeSelectedEventArgs e)
        {
            var tooltip = new ToolTip();
            var edgeView = e.EdgeControl.Edge as EdgeView;
            tooltip.Content = edgeView?.Caption;
            tooltip.IsOpen = true;
            e.EdgeControl.ToolTip = tooltip;
        }

        public static void OnEdgeHoverOut(object sender, EdgeSelectedEventArgs e)
        {
            ((ToolTip)e.EdgeControl.ToolTip).IsOpen = false;
            e.EdgeControl.ToolTip = null;
        }

        public static void OnEdgeRightClick(object sender, EdgeSelectedEventArgs e)
        {
            ContextMenu contextMenu = Application.Current.MainWindow.FindResource("EdgeContextMenu") as ContextMenu;
            contextMenu.PlacementTarget = sender as Button;
            contextMenu.IsOpen = true;
            foreach (Control menuPosition in contextMenu.Items)
            {
                menuPosition.Tag = e.EdgeControl;
            }
        }
    }
}
