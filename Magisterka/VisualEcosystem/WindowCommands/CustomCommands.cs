using System.Windows.Input;
using Magisterka.Domain.Monitoring;
using Magisterka.Infrastructure.RaportGenerating;
using Magisterka.Infrastructure.RaportGenerating.RaportStaticResources;
using Magisterka.VisualEcosystem.Animation;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public static class CustomCommands
    {
        public static ICommand TakePathfindingStepCommand { get; set; }
        public static ICommand StartPathfindingSimulationCommand { get; set; }
        public static ICommand ClearGraphCommand { get; set; }
        public static ICommand AddNewNodeCommand { get; set; }
        public static ICommand AddNewEdgeCommand { get; set; }
        public static ICommand RelayoutGraphCommand { get; set; }
        public static ICommand ToggleNodeDraggingCommand { get; set; }
        public static ICommand ToggleEdgeLabelsCommand { get; set; }
        public static ICommand ToggleEdgeArrowsCommand { get; set; }
        public static ICommand GenerateExcelRaportCommand { get; set; }

        public static void InitilizeCustomCommands(MainWindow window, 
            IMovingActor actor, 
            IAlgorithmMonitor algorithmMonitor, 
            IRaportGenerator raportGenerator, 
            IRaportStringContainerContract raportStringContent)
        {
            TakePathfindingStepCommand = new TakePathfindingStepCommand(window, actor, new CommandValidator());
            StartPathfindingSimulationCommand = new StartPathfindingSimulationCommand(window, actor, new CommandValidator());
            ClearGraphCommand = new ClearGraphCommand(window, actor);
            AddNewNodeCommand = new AddNewNodeCommand(window);
            AddNewEdgeCommand = new AddNewEdgeCommand(window);
            RelayoutGraphCommand = new RelayoutGraphCommand(window);
            ToggleNodeDraggingCommand = new ToggleNodeDraggingCommand(window);
            ToggleEdgeLabelsCommand = new ToggleEdgeLabelsCommand(window);
            ToggleEdgeArrowsCommand = new ToggleEdgeArrowsCommand(window);
            GenerateExcelRaportCommand = new GenerateExcelRaportCommand(algorithmMonitor, raportGenerator, raportStringContent, window);
        }
    }
}
