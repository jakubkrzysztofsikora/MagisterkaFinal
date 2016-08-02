using System.Windows.Input;
using FontAwesome.WPF;
using Magisterka.ViewModels;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class ToggleEdgeLabelsCommand : RoutedUICommand, ICommand
    {
        private readonly MainWindowViewModel _window;

        public ToggleEdgeLabelsCommand(MainWindowViewModel window)
        {
            _window = window;
        }

        public bool CanExecute(object parameter)
        {
            return _window.VisualMap != null && _window.VisualMap.IsLoaded && _window.VisualMap.IsInitialized && _window.VisualMap.LogicCore != null;
        }

        public void Execute(object parameter)
        {
            _window.VisualMap.ShowAllEdgesLabels(!_window.VisualMap.ShowEdgeLabels);
            _window.SetDefaultIcons();
        }
    }
}
