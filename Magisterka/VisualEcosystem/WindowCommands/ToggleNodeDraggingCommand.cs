using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FontAwesome.WPF;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class ToggleNodeDraggingCommand : RoutedUICommand, ICommand
    {
        private readonly MainWindow _window;

        public ToggleNodeDraggingCommand(MainWindow window)
        {
            _window = window;
        }

        public bool CanExecute(object parameter)
        {
            return _window.VisualMap != null && _window.VisualMap.IsLoaded && _window.VisualMap.IsInitialized && _window.VisualMap.LogicCore != null;
        }

        public void Execute(object parameter)
        {
            _window.VisualMap.SetVerticesDrag(!_window.VisualMap.VerticlesDragging);
            _window.DraggingTileIcon.Icon = _window.VisualMap.VerticlesDragging ? FontAwesomeIcon.Unlock : FontAwesomeIcon.Lock;
        }
    }
}
