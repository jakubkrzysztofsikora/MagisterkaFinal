using System;
using System.Linq;
using System.Windows.Input;
using Magisterka.ViewModels;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class ResetSimulationCommand : RoutedUICommand, ICommand
    {
        private readonly MainWindowViewModel _windowViewModel;
        private readonly ICommand _clearGraphCommand;

        public ResetSimulationCommand(MainWindowViewModel windowViewModel, ICommand clearGraphCommand)
        {
            _windowViewModel = windowViewModel;
            _clearGraphCommand = clearGraphCommand;
        }

        public bool CanExecute(object parameter)
        {
            return _clearGraphCommand.CanExecute(parameter) && _windowViewModel.Monitor.PerformanceResults?.MemoryUsageDictionary != null && _windowViewModel.Monitor.PerformanceResults.MemoryUsageDictionary.Any();
        }

        public void Execute(object parameter)
        {
            var startingNode = _windowViewModel.MapAdapter.GetStartNode();
            var targetNode = _windowViewModel.MapAdapter.GetTargetNode();

            _clearGraphCommand.Execute(parameter);

            _windowViewModel.SetStartAndTargetNode(startingNode, targetNode);
            _windowViewModel.HideAlgorithmMonitor();
        }
    }
}
