using System.Windows.Input;
using Magisterka.ViewModels;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class ToggleNodeDraggingCommand : RoutedUICommand, ICommand
    {
        private readonly MainWindowViewModel _window;

        public ToggleNodeDraggingCommand(MainWindowViewModel window)
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
            _window.SetDefaultIcons();
        }
    }
}
