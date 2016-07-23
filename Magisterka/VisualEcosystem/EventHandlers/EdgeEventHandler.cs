using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
