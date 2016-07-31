using System.Windows.Input;
using GraphX.Controls;
using Magisterka.Domain.Adapters;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class AddNewNodeCommand : RoutedUICommand, ICommand
    {
        private MapAdapter _mapAdapter;
        private readonly MainWindow _window;

        public AddNewNodeCommand(MainWindow window)
        {
            _window = window;
        }

        public bool CanExecute(object mapAdapter)
        {
            var adapter = mapAdapter as MapAdapter;
            _mapAdapter = adapter;

            return _mapAdapter != null && _window.IsLoaded;
        }

        public void Execute(object mapAdapter)
        {
            var node =_mapAdapter.AddNode();
            _window.VisualMap.AddVertex(node, new VertexControl(node));
            _window.VisualMap.RefreshGraph();
        }
    }
}
