using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int AnimationsNeededToComplete = 2;
        private readonly IMovingActor _actorToMove;
        private readonly int _defaultBaseFullAnimationLength;
        private List<Point> _allRoutingPoints;

        private int _animationsCompleted;
        private bool _cancelAnimation;
        private EdgeView _throughEdge;
        private Point[] _throughEdgeRoutingPoints;

        private TranslateTransform _transformHandler;
        public MainWindowViewModel _viewModel;

        public PathAnimationCommand(IMovingActor actor, eAnimationSpeed animationSpeed, MainWindowViewModel viewModel)
        {
            _actorToMove = actor;
            _defaultBaseFullAnimationLength = (int) animationSpeed;
            _viewModel = viewModel;
        }

        public VisualMap VisualMap { get; set; }
        public VertexControl FromVertex { get; set; }
        public VertexControl ToVertex { get; set; }
        public event EventHandler AnimationEnded;

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

        public void StopAnimation()
        {
            _transformHandler.BeginAnimation(TranslateTransform.XProperty, null, HandoffBehavior.SnapshotAndReplace);
            _transformHandler.BeginAnimation(TranslateTransform.YProperty, null, HandoffBehavior.SnapshotAndReplace);
            _cancelAnimation = true;
            AnimationEnded = null;
        }

        private void PrepareAnimationComponents(NodeView currentNode, NodeView nextNode)
        {
            var vertexPositions = VisualMap.GetVertexPositions();

            _allRoutingPoints.Insert(0, vertexPositions.Single(nodePostion => nodePostion.Key.LogicNode == currentNode.LogicNode).Value);
            _allRoutingPoints.Add(vertexPositions.Single(nodePostion => nodePostion.Key.LogicNode == nextNode.LogicNode).Value);

            VisualMap.AddCustomChildIfNotExists(_actorToMove.PresentActor(), _allRoutingPoints.First());
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
                animationEnded?.Invoke(_actorToMove.PresentActor(), EventArgs.Empty);
            }
            else if (animationToTransitionalPointEnded && !_cancelAnimation)
            {
                ++iteration;
                Animate(iteration);
            }
        }
    }
}
