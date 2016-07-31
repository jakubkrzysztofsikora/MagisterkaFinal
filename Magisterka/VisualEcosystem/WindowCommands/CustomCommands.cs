using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Magisterka.Domain.Adapters;
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

        public static void InitilizeCustomCommands(MainWindow window, IMovingActor actor)
        {
            TakePathfindingStepCommand = new TakePathfindingStepCommand(window, actor, new CommandValidator());
            StartPathfindingSimulationCommand = new StartPathfindingSimulationCommand(window, actor, new CommandValidator());
            ClearGraphCommand = new ClearGraphCommand(window, actor);
            AddNewNodeCommand = new AddNewNodeCommand(window);
            AddNewEdgeCommand = new AddNewEdgeCommand(window);
            RelayoutGraphCommand = new RelayoutGraphCommand(window);
            ToggleNodeDraggingCommand = new ToggleNodeDraggingCommand(window);
            ToggleEdgeLabelsCommand = new ToggleEdgeLabelsCommand(window);
        }
    }
}
