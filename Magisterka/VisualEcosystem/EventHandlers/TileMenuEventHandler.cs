using System.Windows;
using Magisterka.Domain.Adapters;
using Magisterka.ViewModels;
using MahApps.Metro.Controls;

namespace Magisterka.VisualEcosystem.EventHandlers
{
    public class TileMenuEventHandler
    {
        public MainWindowViewModel MainWindowViewModel { get; set; }

        private readonly MapAdapter _mapAdapter;

        public TileMenuEventHandler(MapAdapter mapAdapter)
        {
            _mapAdapter = mapAdapter;
        }

        public void ClickOnCreateANodeTile(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel.AddNewNodeCommand.CanExecute(_mapAdapter))
                MainWindowViewModel.AddNewNodeCommand.Execute(_mapAdapter);
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
