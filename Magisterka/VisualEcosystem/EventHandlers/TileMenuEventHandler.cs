using System.Windows;
using Magisterka.Domain.Adapters;
using Magisterka.VisualEcosystem.WindowCommands;
using MahApps.Metro.Controls;

namespace Magisterka.VisualEcosystem.EventHandlers
{
    public class TileMenuEventHandler
    {
        private readonly MapAdapter _mapAdapter;

        public TileMenuEventHandler(MapAdapter mapAdapter)
        {
            _mapAdapter = mapAdapter;
        }

        public void ClickOnCreateANodeTile(object sender, RoutedEventArgs e)
        {
            if (CustomCommands.AddNewNodeCommand.CanExecute(_mapAdapter))
                CustomCommands.AddNewNodeCommand.Execute(_mapAdapter);
        }

        public void ClickOnCreateAnEdgeTile(object sender, RoutedEventArgs e)
        {
            if (NodeEventHandler.NewEdgeAdapter == null)
                NodeEventHandler.InitilizeNewEdgeProcess((Tile)sender, _mapAdapter);
            else
                NodeEventHandler.StopNewEdgeProcess();
        }
    }
}
