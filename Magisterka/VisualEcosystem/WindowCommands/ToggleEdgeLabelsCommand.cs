using System.Windows.Input;
using FontAwesome.WPF;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class ToggleEdgeLabelsCommand : RoutedUICommand, ICommand
    {
        private readonly MainWindow _window;

        public ToggleEdgeLabelsCommand(MainWindow window)
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
            _window.EdgeLabelsTileIcon.Icon = _window.VisualMap.ShowEdgeLabels ? FontAwesomeIcon.Eye : FontAwesomeIcon.EyeSlash;
        }
    }
}
