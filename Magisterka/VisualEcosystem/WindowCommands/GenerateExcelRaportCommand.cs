using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IRaportGenerator _raportGenerator;
        private readonly IRaportStringContainerContract _raportStringContent;
        private readonly MainWindowViewModel _window;

        public GenerateExcelRaportCommand(IAlgorithmMonitor algorithmMonitor, 
            IRaportGenerator raportGenerator, 
            IRaportStringContainerContract raportStringContent,
            MainWindowViewModel window)
        {
            _algorithmMonitor = algorithmMonitor;
            _raportGenerator = raportGenerator;
            _raportStringContent = raportStringContent;
            _window = window;
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
            throw new NotImplementedException("Implement showing the message and opening excel file");
            //_window.ShowMessageAsync("Your generated report", $"Your report has been generated under {filePath}");
        }
    }
}
