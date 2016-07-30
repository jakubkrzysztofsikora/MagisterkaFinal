using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Magisterka.Domain.Adapters;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class AddNewEdgeCommand : RoutedUICommand, ICommand
    {
        private MapAdapter _mapAdapter;
        private readonly MainWindow _window;

        public AddNewEdgeCommand(MainWindow window)
        {
            _window = window;
        }

        public bool CanExecute(object mapAdapter)
        {
            var adapter = mapAdapter as MapAdapter;
            _mapAdapter = adapter;

            return _mapAdapter != null && _window.IsLoaded;
        }

        public void Execute(object edgeAdapter)
        {
            var adapter = edgeAdapter as EdgeAdapter;
            throw new NotImplementedException();
        }
    }
}
