using System.Windows.Input;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.ViewModels;
using Magisterka.ViewModels;
using Magisterka.VisualEcosystem.Animation;
using Magisterka.VisualEcosystem.Animation.AnimationCommands;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class TakePathfindingStepCommand : RoutedUICommand,ICommand
    {
        private MapAdapter _mapAdapter;
        private readonly MainWindowViewModel _applicationWindow;
        private readonly IMovingActor _animatingActor;
        private readonly ICommandValidator _validator;

        public TakePathfindingStepCommand(MainWindowViewModel applicationWindow, 
            IMovingActor animatingActor, 
            ICommandValidator validator) 
            : base("Take pathfinding step", "TakePathfindingStep", typeof(TakePathfindingStepCommand), new InputGestureCollection
        {
            new KeyGesture(Key.F5, ModifierKeys.None)
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
            return _applicationWindow.VisualMap != null && _applicationWindow.VisualMap.IsLoaded && _mapAdapter != null && _mapAdapter.CanStartPathfinding();
        }

        public void Execute(object parameter)
        {
            var enumParams = parameter as object[];
            _validator.ValidateConfiguration(_mapAdapter, enumParams);
            var algorithm = (ePathfindingAlgorithms)enumParams[0];
            var animationSpeed = (eAnimationSpeed)enumParams[1];

            VertexControl currentVertex = _applicationWindow.VisualMap.GetCurrentVertex();

            NodeView nextNode = _mapAdapter.StartPathfindingByStep(currentVertex.GetNodeView(), algorithm);

            VertexControl nextVertexControl = _applicationWindow.VisualMap.GetVertexControlOfNode(nextNode);

            var animation = new PathAnimationCommand(_animatingActor, animationSpeed, _applicationWindow)
            {
                FromVertex = currentVertex,
                ToVertex = nextVertexControl,
                VisualMap = _applicationWindow.VisualMap
            };

            _applicationWindow.VisualMap.GoToVertex(nextVertexControl, animation);
        }
    }
}
