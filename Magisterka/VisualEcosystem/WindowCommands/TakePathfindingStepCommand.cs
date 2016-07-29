using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.ExceptionContracts;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.Animation;
using Magisterka.VisualEcosystem.Animation.AnimationCommands;
using Magisterka.VisualEcosystem.ErrorHandling;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class TakePathfindingStepCommand : ICommand
    {
        private readonly MapAdapter _mapAdapter;
        private readonly VisualMap _frontGraph;
        private readonly IMovingActor _animatingActor;

        public TakePathfindingStepCommand(MapAdapter mapAdapter, 
            VisualMap frontGraph, 
            IMovingActor animatingActor)
        {
            _mapAdapter = mapAdapter;
            _frontGraph = frontGraph;
            _animatingActor = animatingActor;
        }

        public bool CanExecute(object parameter)
        {
            return true;//todo: temp, change to checking entry conditions
        }

        public void Execute(object parameter)
        {
            VertexControl currentVertex = _frontGraph.GetCurrentVertex();

            NodeView nextNode = _mapAdapter.StartPathfinding(currentVertex.GetNodeView(), (ePathfindingAlgorithms)parameter);

            VertexControl nextVertexControl = _frontGraph.GetVertexControlOfNode(nextNode);

            var animation = new PathAnimationCommand(_animatingActor, eAnimationSpeed.Fast)
            {
                FromVertex = currentVertex,
                ToVertex = nextVertexControl,
                VisualMap = _frontGraph
            };

            _frontGraph.GoToVertex(nextVertexControl, animation);
        }

        public event EventHandler CanExecuteChanged;
    }
}
