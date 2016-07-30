using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Castle.Core.Internal;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.Animation;
using Magisterka.VisualEcosystem.Animation.AnimationCommands;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class StartPathfindingSimulationCommand : RoutedUICommand, ICommand
    {
        private MapAdapter _mapAdapter;
        private readonly MainWindow _applicationWindow;
        private readonly IMovingActor _animatingActor;
        private readonly CommandValidator _validator;

        public StartPathfindingSimulationCommand(MainWindow applicationWindow,
            IMovingActor animatingActor,
            CommandValidator validator)
            : base("Start pathfinding simulation", "StartPathfindingSimulation", typeof(TakePathfindingStepCommand), new InputGestureCollection
        {
            new KeyGesture(Key.F5, ModifierKeys.Control)
        })
        {
            _applicationWindow = applicationWindow;
            _animatingActor = animatingActor;
            _validator = validator;
        }
        public bool CanExecute(object mapAdapter)
        {
            var adapter = mapAdapter as MapAdapter;
            _mapAdapter = adapter;
            return CustomCommands.TakePathfindingStepCommand.CanExecute(mapAdapter);
        }

        public void Execute(object parameter)
        {
            var enumParams = parameter as object[];
            _validator.ValidateConfiguration(_mapAdapter, enumParams);
            
            var algorithm = (ePathfindingAlgorithms)enumParams[0];
            var animationSpeed = (eAnimationSpeed)enumParams[1];

            VertexControl currentVertex = _applicationWindow.VisualMap.GetCurrentVertex();
            
            AnimatePath(currentVertex, algorithm, animationSpeed);
        }

        private void AnimatePath(VertexControl startingVertex, ePathfindingAlgorithms algorithm, eAnimationSpeed animationSpeed, int iteration = 0)
        {
            IEnumerable<NodeView> path = _mapAdapter.StartPathfindingAllRoute(startingVertex.GetNodeView(), algorithm);
            var enumeratedPath = path.ToList();

            if (enumeratedPath.Count <= iteration)
                return;

            VertexControl currentVertex = _applicationWindow.VisualMap.GetCurrentVertex();
            VertexControl nextVertexControl = _applicationWindow.VisualMap.GetVertexControlOfNode(enumeratedPath[iteration]);

            var animation = new PathAnimationCommand(_animatingActor, animationSpeed)
            {
                FromVertex = currentVertex,
                ToVertex = nextVertexControl,
                VisualMap = _applicationWindow.VisualMap
            };

            _applicationWindow.VisualMap.GoToVertex(nextVertexControl, animation);
            animation.AnimationEnded += delegate
            {
                AnimatePath(startingVertex, algorithm, animationSpeed, iteration + 1);
            };
        }
    }
}
