using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.ExceptionContracts;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.ViewModels;
using Magisterka.ViewModels;
using Magisterka.VisualEcosystem.Animation;
using Magisterka.VisualEcosystem.Animation.AnimationCommands;
using Magisterka.VisualEcosystem.ErrorHandling;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class StartPathfindingSimulationCommand : RoutedUICommand, ICommand
    {
        private readonly IMovingActor _animatingActor;
        private readonly MainWindowViewModel _applicationWindow;
        private readonly ICommandValidator _validator;
        private readonly IErrorDisplayer _errorDisplayer;
        private bool _commandStopped;
        private MapAdapter _mapAdapter;

        public StartPathfindingSimulationCommand(MainWindowViewModel applicationWindow,
            IMovingActor animatingActor,
            ICommandValidator validator,
            IErrorDisplayer errorDisplayer)
            : base("Start pathfinding simulation", "StartPathfindingSimulation", typeof(TakePathfindingStepCommand), new InputGestureCollection
        {
            new KeyGesture(Key.F5, ModifierKeys.Control)
        })
        {
            _applicationWindow = applicationWindow;
            _animatingActor = animatingActor;
            _validator = validator;
            _errorDisplayer = errorDisplayer;

            _applicationWindow.ClearedGraph += OnClearedGraph;
        }

        public bool CanExecute(object mapAdapter)
        {
            var adapter = mapAdapter as MapAdapter;
            _mapAdapter = adapter;
            return _applicationWindow.VisualMap != null && _applicationWindow.VisualMap.IsLoaded && _mapAdapter != null && _mapAdapter.CanStartPathfinding();
        }

        public void Execute(object parameter)
        {
            _commandStopped = false;
            var enumParams = parameter as object[];
            _validator.ValidateConfiguration(_mapAdapter, enumParams);
            _applicationWindow.DisplayAlgorithmMonitor();
            
            var algorithm = (ePathfindingAlgorithms)enumParams[0];
            var animationSpeed = (eAnimationSpeed)enumParams[1];

            VertexControl currentVertex = _applicationWindow.VisualMap.GetCurrentVertex();
            
            AnimatePath(currentVertex, algorithm, animationSpeed);
        }

        private void OnClearedGraph(object sender, EventArgs eventArgs)
        {
            _commandStopped = true;
        }

        private void AnimatePath(VertexControl startingVertex, ePathfindingAlgorithms algorithm, eAnimationSpeed animationSpeed, int iteration = 0)
        {
            IEnumerable<NodeView> path = _mapAdapter.StartPathfindingAllRoute(startingVertex.GetNodeView(), algorithm);

            var enumeratedPath = path.ToList();

            if (enumeratedPath.Count <= iteration || _commandStopped)
                return;

            VertexControl currentVertex = _applicationWindow.VisualMap.GetCurrentVertex();
            VertexControl nextVertexControl = _applicationWindow.VisualMap.GetVertexControlOfNode(enumeratedPath[iteration]);

            var animation = new PathAnimationCommand(_animatingActor, animationSpeed, _applicationWindow)
            {
                FromVertex = currentVertex,
                ToVertex = nextVertexControl,
                VisualMap = _applicationWindow.VisualMap
            };

            _applicationWindow.VisualMap.GoToVertex(nextVertexControl, animation);


            animation.AnimationEnded += (sender, args) =>
            {
                try
                {
                    AnimatePath(startingVertex, algorithm, animationSpeed, iteration + 1);
                }
                catch (DomainException exception)
                {
                    _errorDisplayer.DisplayError(exception.ErrorType, exception.Message);
                }
                catch (Exception exception)
                {
                    _errorDisplayer.DisplayError(eErrorTypes.General, exception.Message);
                }
            };
        }
    }
}
