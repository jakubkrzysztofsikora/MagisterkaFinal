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

        public static void InitilizeCustomCommands(MainWindow frontGraph, IMovingActor actor)
        {
            TakePathfindingStepCommand = new TakePathfindingStepCommand(frontGraph, actor);
        }
    }
}
