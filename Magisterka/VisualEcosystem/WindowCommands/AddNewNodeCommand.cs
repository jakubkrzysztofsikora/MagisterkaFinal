using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DocumentFormat.OpenXml.Drawing;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

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
            _window.VisualMap.RefreshGraph();
        }
    }
}
