using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphX;
using GraphX.Controls;
using Magisterka.ViewModels;

namespace Magisterka.VisualEcosystem.Animation.AnimationCommands
{
    public class NewEdgeMouseAnimationCommand : IAnimationCommand
    {
        private readonly Rectangle _edgeShape;
        private readonly VertexControl _fromVertexControl;
        private readonly Point _initialPointOfClick;
        private readonly MainWindowViewModel _viewModel;

        public NewEdgeMouseAnimationCommand(VertexControl fromVertexControl, MainWindowViewModel viewModel, MouseButtonEventArgs initialMouseArgs)
        {
            _fromVertexControl = fromVertexControl;
            _viewModel = viewModel;
            _initialPointOfClick = initialMouseArgs.GetPosition(_viewModel.VisualMap);
            _edgeShape = new Rectangle
            {
                Width = 3,
                Height = 1,
                Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                Opacity = 0.5
            };
        }

        public event EventHandler AnimationEnded;

        public void BeginAnimation()
        {
            _viewModel.ZoomControl.MouseMove += OnMouseMoveDuringCreationOfEdge;
            var vertexPosition = _initialPointOfClick;

            _viewModel.VisualMap.AddCustomChildIfNotExists(_edgeShape);
            PositionEdgeShapeOnCanvas(vertexPosition);
        }

        public void StopAnimation()
        {
            _viewModel.ZoomControl.MouseMove -= OnMouseMoveDuringCreationOfEdge;
            _viewModel.VisualMap.Children.Remove(_edgeShape);
            AnimationEnded?.Invoke(_fromVertexControl, EventArgs.Empty);
        }

        private void PositionEdgeShapeOnCanvas(Point location)
        {
            _edgeShape.SetValue(GraphAreaBase.XProperty, location.X);
            _edgeShape.SetValue(GraphAreaBase.YProperty, location.Y);
            Canvas.SetTop(_edgeShape, location.Y);
            Canvas.SetLeft(_edgeShape, location.X);
        }

        private void OnMouseMoveDuringCreationOfEdge(object sender, MouseEventArgs mouseEventArgs)
        {
            var mousePosition = mouseEventArgs.GetPosition(_viewModel.VisualMap);
            var mouseVertexDiagonal = Math.Sqrt(Math.Pow(mousePosition.Y - _initialPointOfClick.Y, 2) + Math.Pow(mousePosition.X - _initialPointOfClick.X, 2));
            var mouseToVertexAngleRadians = Math.Asin(Math.Abs(mousePosition.X - _initialPointOfClick.X) / mouseVertexDiagonal);

            var mouseToVertexAngleDegrees = GetDegreesFromRadians(mouseToVertexAngleRadians, mousePosition);

            _edgeShape.RenderTransformOrigin = new Point(0.5, 0);
            _edgeShape.RenderTransform = new RotateTransform(mouseToVertexAngleDegrees);

            _edgeShape.Height = mouseVertexDiagonal <= 5 ? 1 : mouseVertexDiagonal - 5;
        }

        private double GetDegreesFromRadians(double radians, Point currentMousePosition)
        {
            var degrees = radians * (180.0 / Math.PI);
            bool mouseInSecondQuarter = currentMousePosition.X - _initialPointOfClick.X <= 0 && currentMousePosition.Y - _initialPointOfClick.Y <= 0;
            bool mouseInThirdQuarter = currentMousePosition.X - _initialPointOfClick.X >= 0 && currentMousePosition.Y - _initialPointOfClick.Y <= 0;
            bool mouseInFourthQuarter = currentMousePosition.X - _initialPointOfClick.X >= 0 && currentMousePosition.Y - _initialPointOfClick.Y >= 0;

            if (mouseInSecondQuarter)
                degrees = 90 + (90 - degrees);
            else if (mouseInThirdQuarter)
                degrees += 180;
            else if (mouseInFourthQuarter)
                degrees = 360 - degrees;

            return degrees;
        }
    }
}
