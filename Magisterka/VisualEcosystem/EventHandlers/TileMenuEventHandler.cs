using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.VisualEcosystem.WindowCommands;

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
    }
}
