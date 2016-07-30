using System.Windows.Input;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.Animation;
using Magisterka.VisualEcosystem.Animation.AnimationCommands;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class TakePathfindingStepCommand : RoutedUICommand,ICommand
    {
        private MapAdapter _mapAdapter;
        private readonly MainWindow _applicationWindow;
        private readonly IMovingActor _animatingActor;

        public TakePathfindingStepCommand(MainWindow applicationWindow, 
            IMovingActor animatingActor) : base("Take pathfinding step", "TakePathfindingStep", typeof(TakePathfindingStepCommand), new InputGestureCollection
        {
            new KeyGesture(Key.F5, ModifierKeys.None)
        })
        {
            _applicationWindow = applicationWindow;
            _animatingActor = animatingActor;
        }

        public bool CanExecute(object mapAdapter)
        {
            var adapter = mapAdapter as MapAdapter;
            _mapAdapter = adapter;
            return _applicationWindow.VisualMap != null && _applicationWindow.VisualMap.IsLoaded && _mapAdapter != null && _mapAdapter.CanStartPathfinding();
        }

        public void Execute(object parameter)
        {
            VertexControl currentVertex = _applicationWindow.VisualMap.GetCurrentVertex();

            NodeView nextNode = _mapAdapter.StartPathfinding(currentVertex.GetNodeView(), (ePathfindingAlgorithms)parameter);

            VertexControl nextVertexControl = _applicationWindow.VisualMap.GetVertexControlOfNode(nextNode);

            var animation = new PathAnimationCommand(_animatingActor, eAnimationSpeed.Fast)
            {
                FromVertex = currentVertex,
                ToVertex = nextVertexControl,
                VisualMap = _applicationWindow.VisualMap
            };

            _applicationWindow.VisualMap.GoToVertex(nextVertexControl, animation);
        }
    }
}
