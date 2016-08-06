using System.Windows;
using System.Windows.Controls;
using GraphX.Controls;
using GraphX.Controls.Models;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.ViewModels;
using Magisterka.ViewModels;
using Magisterka.VisualEcosystem.Animation.AnimationCommands;
using Magisterka.VisualEcosystem.Extensions;
using MahApps.Metro.Controls;

namespace Magisterka.VisualEcosystem.EventHandlers
{
    public class NodeEventHandler
    {
        private static Tile _clickedTile;
        private static NewEdgeMouseAnimationCommand _edgeAnimation;
        public static MainWindowViewModel MainWindowViewModel { get; set; }
        public static string NameOfNodeContextMenu { get; set; }
        public static string NameOfSetAsBlocked { get; set; }
        public static string NameOfSetAsUnblocked { get; set; }
        public static EdgeAdapter NewEdgeAdapter { get; set; }

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

        public static void OnNodeMouseDown(object sender, VertexSelectedEventArgs e)
        {
            if (NewEdgeAdapter == null)
                return;

            if (_edgeAnimation == null)
            {
                _edgeAnimation = new NewEdgeMouseAnimationCommand(e.VertexControl, MainWindowViewModel, e.MouseArgs);
                _edgeAnimation.BeginAnimation();
            }

            if (NewEdgeAdapter.FromNode == null)
                NewEdgeAdapter.FromNode = e.VertexControl.GetNodeView().LogicNode;
            else
            {
                NewEdgeAdapter.ToNode = e.VertexControl.GetNodeView().LogicNode;

                if (MainWindowViewModel.AddNewEdgeCommand.CanExecute(NewEdgeAdapter))
                    MainWindowViewModel.AddNewEdgeCommand.Execute(NewEdgeAdapter);
                StopNewEdgeProcess();
            }
        }

        public static void OnNodeRightClick(object sender, VertexSelectedEventArgs e)
        {
            ContextMenu contextMenu = Application.Current.MainWindow.FindResource(NameOfNodeContextMenu) as ContextMenu;
            contextMenu.PlacementTarget = sender as Button;
            contextMenu.IsOpen = true;
            AddVertexControlsToMenuTags(contextMenu, e.VertexControl);

            SetMenuPositionDependantOnProperty(contextMenu, e.VertexControl.GetNodeView().LogicNode.IsBlocked, NameOfSetAsBlocked);
            SetMenuPositionDependantOnProperty(contextMenu, !e.VertexControl.GetNodeView().LogicNode.IsBlocked, NameOfSetAsUnblocked);
        }

        public static void InitilizeNewEdgeProcess(Tile clickedTile, MapAdapter mapAdapter)
        {
            NewEdgeAdapter = new EdgeAdapter
            {
                MapAdapter = mapAdapter
            };
            clickedTile.Title = "Cancel";
            _clickedTile = clickedTile;
            
            if (MainWindowViewModel.VisualMap.VerticlesDragging)
                MainWindowViewModel.ToggleNodeDraggingCommand.Execute(null);
        }

        public static void StopNewEdgeProcess()
        {
            NewEdgeAdapter = null;
            _clickedTile.Title = "New Edge";
            _edgeAnimation?.StopAnimation();
            _edgeAnimation = null;

            if (!MainWindowViewModel.VisualMap.VerticlesDragging)
                MainWindowViewModel.ToggleNodeDraggingCommand.Execute(null);
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
