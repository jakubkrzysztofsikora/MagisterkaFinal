using System;
using System.Windows.Input;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class RelayoutGraphCommand : RoutedUICommand, ICommand
    {
        private readonly MainWindow _window;

        public RelayoutGraphCommand(MainWindow window)
        {
            _window = window;
        }

        public bool CanExecute(object parameter)
        {
            return _window.VisualMap != null && _window.VisualMap.IsLoaded && _window.VisualMap.IsInitialized && _window.VisualMap.LogicCore != null;
        }

        public void Execute(object parameter)
        {
            EventHandler handlerOfFinishedCalculation = (sender, args) => _window.ZoomControl.ZoomToFill();
            _window.VisualMap.LayoutUpdated += handlerOfFinishedCalculation;

            _window.VisualMap.RelayoutGraph(true);
        }
    }
}
