using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GraphX.Controls;
using GraphX.Measure;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.Extensions;

namespace Magisterka.VisualEcosystem.Animation.AnimationCommands
{
    public class PathAnimationCommand : IAnimationCommand
    {
        private const int AnimationsNeededToComplete = 2;
        private readonly IMovingActor _actorToMove;
        private readonly int _defaultBaseFullAnimationLength;

        private int _animationsCompleted;
        private List<Point> _routingPoints;

        private EdgeView _throughEdge;

        public PathAnimationCommand(IMovingActor actor, eAnimationSpeed animationSpeed)
        {
            _actorToMove = actor;
            _defaultBaseFullAnimationLength = (int) animationSpeed;
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
            _routingPoints = new List<Point>(_throughEdge.RoutingPoints);

            PrepareAnimationComponents(currentNode, nextNode);
            
            Animate();
        }

        private void PrepareAnimationComponents(NodeView currentNode, NodeView nextNode)
        {
            var vertexPositions = VisualMap.GetVertexPositions();
            VisualMap.AddCustomChildIfNotExists(_actorToMove.PresentActor());

            _routingPoints.Insert(0, vertexPositions.Single(nodePostion => nodePostion.Key.LogicNode == currentNode.LogicNode).Value);
            _routingPoints.Add(vertexPositions.Single(nodePostion => nodePostion.Key.LogicNode == nextNode.LogicNode).Value);
                

            Canvas.SetLeft(_actorToMove.PresentActor(), _routingPoints.First().X);
            Canvas.SetTop(_actorToMove.PresentActor(), _routingPoints.First().Y);
        }

        private void Animate(int iteration = 0)
        {
            _animationsCompleted = 0;
            TranslateTransform trans = new TranslateTransform();
            _actorToMove.PresentActor().RenderTransform = trans;

            Point currentActorPoint = iteration == 0
                ? _routingPoints.First()
                : _routingPoints[iteration - 1];
            Point nextActorPoint = _throughEdge.RoutingPoints.Length > iteration
                ? _routingPoints[iteration]
                : _routingPoints.Last();

            int partialAnimationLength = _defaultBaseFullAnimationLength / _routingPoints.Count * _throughEdge.LogicEdge.Cost;
            DoubleAnimation anim1 = new DoubleAnimation(currentActorPoint.X, nextActorPoint.X, TimeSpan.FromMilliseconds(partialAnimationLength));
            DoubleAnimation anim2 = new DoubleAnimation(currentActorPoint.Y, nextActorPoint.Y, TimeSpan.FromMilliseconds(partialAnimationLength));
            anim1.Completed += (sender, args) => FireCompletedEventIfPossible(iteration);
            anim2.Completed += (sender, args) => FireCompletedEventIfPossible(iteration);

            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);
        }

        private void FireCompletedEventIfPossible(int iteration)
        {
            ++_animationsCompleted;
            
            bool animationToTransitionalPointEnded = _animationsCompleted == AnimationsNeededToComplete;
            bool finalDestinationReached = animationToTransitionalPointEnded && _throughEdge.RoutingPoints.Length == iteration;

            if (finalDestinationReached)
            {
                EventHandler animationEnded = AnimationEnded?.Clone() as EventHandler;
                animationEnded?.Invoke(new object(), EventArgs.Empty);
            }
            else if (animationToTransitionalPointEnded)
            {
                ++iteration;
                Animate(iteration);
            }
        }
    }
}
