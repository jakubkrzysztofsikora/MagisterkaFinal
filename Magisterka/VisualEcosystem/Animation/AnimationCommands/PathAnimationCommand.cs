using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GraphX.Controls;
using GraphX.Measure;
using Magisterka.Domain.ExceptionContracts;
using Magisterka.Domain.ViewModels;
using Magisterka.ViewModels;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.Animation.AnimationCommands
{
    public class PathAnimationCommand : IAnimationCommand
    {
        public MainWindowViewModel _viewModel;
        public VisualMap VisualMap { get; set; }
        public VertexControl FromVertex { get; set; }
        public VertexControl ToVertex { get; set; }
        public event EventHandler AnimationEnded;

        private const int AnimationsNeededToComplete = 2;
        private readonly IMovingActor _actorToMove;
        private readonly int _defaultBaseFullAnimationLength;

        private int _animationsCompleted;
        private List<Point> _allRoutingPoints;
        private bool _cancelAnimation;

        private TranslateTransform _transformHandler;
        private EdgeView _throughEdge;
        private Point[] _throughEdgeRoutingPoints;

        public PathAnimationCommand(IMovingActor actor, eAnimationSpeed animationSpeed, MainWindowViewModel viewModel)
        {
            _actorToMove = actor;
            _defaultBaseFullAnimationLength = (int) animationSpeed;
            _viewModel = viewModel;
            _viewModel.ClearedGraph += OnGraphClearDuringAnimation;
        }

        public void BeginAnimation()
        {
            NodeView currentNode = FromVertex.GetNodeView();
            NodeView nextNode = ToVertex.GetNodeView();
            _throughEdge = VisualMap.GetEdgeControlBetweenNodes(currentNode, nextNode).GetEdgeView();

            if (_throughEdge == null)
                throw new DomainException("There is no direct edge connection between the nodes.")
                {
                    ErrorType = eErrorTypes.PathfindingError
                };

            _throughEdgeRoutingPoints = _throughEdge.RoutingPoints ?? new Point[] {};
            _allRoutingPoints = new List<Point>(_throughEdgeRoutingPoints);

            PrepareAnimationComponents(currentNode, nextNode);
            
            Animate();
        }

        private void OnGraphClearDuringAnimation(object sender, EventArgs eventArgs)
        {
            _transformHandler.BeginAnimation(TranslateTransform.XProperty, null, HandoffBehavior.SnapshotAndReplace);
            _transformHandler.BeginAnimation(TranslateTransform.YProperty, null, HandoffBehavior.SnapshotAndReplace);
            _cancelAnimation = true;
            _viewModel.ClearedGraph -= OnGraphClearDuringAnimation;
        }

        private void PrepareAnimationComponents(NodeView currentNode, NodeView nextNode)
        {
            var vertexPositions = VisualMap.GetVertexPositions();
            VisualMap.AddCustomChildIfNotExists(_actorToMove.PresentActor());

            _allRoutingPoints.Insert(0, vertexPositions.Single(nodePostion => nodePostion.Key.LogicNode == currentNode.LogicNode).Value);
            _allRoutingPoints.Add(vertexPositions.Single(nodePostion => nodePostion.Key.LogicNode == nextNode.LogicNode).Value);
                

            Canvas.SetLeft(_actorToMove.PresentActor(), _allRoutingPoints.First().X);
            Canvas.SetTop(_actorToMove.PresentActor(), _allRoutingPoints.First().Y);
        }

        private void Animate(int iteration = 0)
        {
            if (_cancelAnimation)
                return;

            _animationsCompleted = 0;
            _transformHandler = new TranslateTransform();
            _actorToMove.PresentActor().RenderTransform = _transformHandler;

            Point currentActorPoint = iteration == 0
                ? _allRoutingPoints.First()
                : _allRoutingPoints[iteration - 1];
            Point nextActorPoint = _allRoutingPoints.Count > iteration + 1
                ? _allRoutingPoints[iteration]
                : _allRoutingPoints.Last();

            int partialAnimationLength = _defaultBaseFullAnimationLength / _allRoutingPoints.Count * _throughEdge.LogicEdge.Cost;
            DoubleAnimation anim1 = new DoubleAnimation(currentActorPoint.X, nextActorPoint.X, TimeSpan.FromMilliseconds(partialAnimationLength));
            DoubleAnimation anim2 = new DoubleAnimation(currentActorPoint.Y, nextActorPoint.Y, TimeSpan.FromMilliseconds(partialAnimationLength));
            anim1.Completed += (sender, args) => FireCompletedEventIfPossible(iteration);
            anim2.Completed += (sender, args) => FireCompletedEventIfPossible(iteration);

            _transformHandler.BeginAnimation(TranslateTransform.XProperty, anim1, HandoffBehavior.Compose);
            _transformHandler.BeginAnimation(TranslateTransform.YProperty, anim2, HandoffBehavior.Compose);
        }

        private void FireCompletedEventIfPossible(int iteration)
        {
            ++_animationsCompleted;
            
            bool animationToTransitionalPointEnded = _animationsCompleted == AnimationsNeededToComplete;
            bool finalDestinationReached = animationToTransitionalPointEnded && _allRoutingPoints.Count == iteration + 1;

            if (finalDestinationReached)
            {
                EventHandler animationEnded = AnimationEnded?.Clone() as EventHandler;
                animationEnded?.Invoke(new object(), EventArgs.Empty);
                _viewModel.ClearedGraph -= OnGraphClearDuringAnimation;
            }
            else if (animationToTransitionalPointEnded && !_cancelAnimation)
            {
                ++iteration;
                Animate(iteration);
            }
        }
    }
}
