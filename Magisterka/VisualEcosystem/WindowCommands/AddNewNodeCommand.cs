using System.Windows.Input;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.ViewModels;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class AddNewNodeCommand : RoutedUICommand, ICommand
    {
        private readonly MainWindowViewModel _window;
        private MapAdapter _mapAdapter;

        public AddNewNodeCommand(MainWindowViewModel window)
        {
            _window = window;
        }

        public bool CanExecute(object mapAdapter)
        {
            var adapter = mapAdapter as MapAdapter;
            _mapAdapter = adapter;

            return _mapAdapter != null && _window.VisualMap.IsLoaded;
        }

        public void Execute(object mapAdapter)
        {
            var node =_mapAdapter.AddNode();
            var newVertex = new VertexControl(node);
            _window.VisualMap.AddVertex(node);
            newVertex.SetPosition(0,0);
            _window.VisualMap.RefreshGraph();
        }
    }
}
