using System.Windows.Input;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using Magisterka.Infrastructure.RaportGenerating;
using Magisterka.Infrastructure.RaportGenerating.RaportStaticResources;
using Magisterka.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class GenerateExcelRaportCommand : RoutedUICommand, ICommand
    {
        private readonly IAlgorithmMonitor _algorithmMonitor;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IRaportGenerator _raportGenerator;
        private readonly IRaportStringContainerContract _raportStringContent;
        private readonly MainWindowViewModel _viewModel;

        public GenerateExcelRaportCommand(IAlgorithmMonitor algorithmMonitor, 
            IRaportGenerator raportGenerator, 
            IRaportStringContainerContract raportStringContent,
            IDialogCoordinator dialogCoordinator,
            MainWindowViewModel viewModel)
        {
            _algorithmMonitor = algorithmMonitor;
            _raportGenerator = raportGenerator;
            _raportStringContent = raportStringContent;
            _dialogCoordinator = dialogCoordinator;
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return _algorithmMonitor?.PathDetails != null && _algorithmMonitor.PerformanceResults != null;
        }

        public void Execute(object parameter)
        {
            var enumParams = parameter as object[];
            var algorithm = (ePathfindingAlgorithms)enumParams[0];
            string filePath = _raportGenerator.GenerateRaport(new ExcelRaportCommand
            {
                AlgorithmMonitor = _algorithmMonitor,
                PathfindingAlgorithm = algorithm,
                RaportStrings = _raportStringContent
            });

            _dialogCoordinator.ShowMessageAsync(_viewModel, "Your generated report", $"Your report has been generated under {filePath}");
        }
    }
}
