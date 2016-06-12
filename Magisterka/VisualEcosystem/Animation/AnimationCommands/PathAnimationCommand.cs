using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GraphX.Controls;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.Extensions;
using Point = GraphX.Measure.Point;

namespace Magisterka.VisualEcosystem.Animation.AnimationCommands
{
    public class PathAnimationCommand : IAnimationCommand
    {
        public event EventHandler AnimationEnded;

        public VisualMap VisualMap { get; set; }
        public VertexControl FromVertex { get; set; }
        public VertexControl ToVertex { get; set; }

        private const int AnimationsNeededToComplete = 2;

        private Point _currentVertexPosition;
        private Point _nextVertexPosition;
        private EdgeView _throughEdge;
        private readonly UIElement _actorToMove;
        private int _animationsCompleted;

        public PathAnimationCommand(IMovingActor actor)
        {
            _actorToMove = actor.PresentActor();
        }

        public void BeginAnimation()
        {
            _animationsCompleted = 0;
            NodeView currentNode = FromVertex.GetNodeView();
            NodeView nextNode = ToVertex.GetNodeView();
            _throughEdge = VisualMap.GetEdgeControlBetweenNodes(currentNode, nextNode).GetEdgeView();

            PrepareAnimationComponents(currentNode, nextNode);
            Animate();
        }

        private void PrepareAnimationComponents(NodeView currentNode, NodeView nextNode)
        {
            var vertexPositions = VisualMap.GetVertexPositions();
            
            VisualMap.AddCustomChildIfNotExists(_actorToMove);
            _currentVertexPosition =
                vertexPositions.Single(nodePostion => nodePostion.Key.LogicNode == currentNode.LogicNode).Value;
            _nextVertexPosition =
                vertexPositions.Single(nodePostion => nodePostion.Key.LogicNode == nextNode.LogicNode).Value;
            Canvas.SetLeft(_actorToMove, _currentVertexPosition.X);
            Canvas.SetTop(_actorToMove, _currentVertexPosition.Y);
        }

        private void Animate()
        {
            TranslateTransform trans = new TranslateTransform();
            _actorToMove.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(_currentVertexPosition.X, _nextVertexPosition.X, TimeSpan.FromSeconds(2));
            anim1.Completed += (sender, args) => FireCompletedEventIfPossible();
            DoubleAnimation anim2 = new DoubleAnimation(_currentVertexPosition.Y, _nextVertexPosition.Y, TimeSpan.FromSeconds(2));
            anim2.Completed += (sender, args) => FireCompletedEventIfPossible();
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);
        }

        private void FireCompletedEventIfPossible()
        {
            ++_animationsCompleted;

            if (_animationsCompleted == AnimationsNeededToComplete)
            {
                EventHandler animationEnded = AnimationEnded?.Clone() as EventHandler;
                animationEnded?.Invoke(new object(), EventArgs.Empty);
            }
        }
    }
}
