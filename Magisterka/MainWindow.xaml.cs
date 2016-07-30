using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphX.Controls;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.ExceptionContracts;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.Animation;
using Magisterka.VisualEcosystem.Animation.AnimationCommands;
using Magisterka.VisualEcosystem.ErrorHandling;
using Magisterka.VisualEcosystem.EventHandlers;
using Magisterka.VisualEcosystem.Extensions;
using Magisterka.VisualEcosystem.InputModals;
using Magisterka.VisualEcosystem.Validators;
using Magisterka.VisualEcosystem.WindowCommands;
using MahApps.Metro.Controls;

namespace Magisterka
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public IAlgorithmMonitor Monitor { get; private set; }

        private readonly IMovingActor _actor;
        private readonly Random _randomizer;
        private readonly IErrorDisplayer _errorDisplayer;
        private readonly IConfigurationValidator _validator;
        private readonly IMapFactory _mapFactory;
        private readonly IPathfinderFactory _pathfinderFactory;
        private MapAdapter _mapAdapter;

        public MainWindow(IErrorDisplayer errorDisplayer, 
                          IConfigurationValidator validator,
                          IMapFactory mapFactory,
                          IPathfinderFactory pathfinderFactory,
                          IMovingActor actor,
                          IAlgorithmMonitor monitor,
                          Random randomizer)
        {
            _errorDisplayer = errorDisplayer;
            _validator = validator;
            _mapFactory = mapFactory;
            _pathfinderFactory = pathfinderFactory;
            _actor = actor;
            _randomizer = randomizer;
            Monitor = monitor;

            CustomCommands.InitilizeCustomCommands(this, _actor);
            InitializeComponent();
        }

        public void Dispose()
        {
            VisualMap.Dispose();
        }

        private void InitializeEventHandlers()
        {
            VisualMap.VertexMouseEnter += NodeEventHandler.OnNodeHoverIn;
            VisualMap.VertexMouseLeave += NodeEventHandler.OnNodeHoverOut;
            VisualMap.VertexRightClick += NodeEventHandler.OnNodeRightClick;

            VisualMap.EdgeMouseEnter += EdgeEventHandler.OnEdgeHoverIn;
            VisualMap.EdgeMouseLeave += EdgeEventHandler.OnEdgeHoverOut;
            VisualMap.EdgeRightClick += EdgeEventHandler.OnEdgeRightClick;

            VisualMap.GenerateGraphFinished += (eAnimationSpeed, args) => LoadingOff();
            
            SizeChanged += (e,args) => ZoomControl.ZoomToFill();
        }

        private void LoadingOn()
        {
            ZoomControl.Visibility = Visibility.Hidden;
            ProgressRing.IsActive = true;
        }

        private void LoadingOff()
        {
            ZoomControl.Visibility = Visibility.Visible;
            ProgressRing.IsActive = false;
            ZoomControl.ZoomToFill();
        }

        private void CreateAllLayersOfGraph(IMapFactory mapFactory, Random randomizer, IPathfinderFactory pathfinderFactory)
        {
            var map = mapFactory.GenerateDefaultMap()
                .WithGridPositions()
                .WithRandomBlockedNodes(randomizer);
            _mapAdapter= MapAdapter.CreateMapAdapterFromLogicMap(map, pathfinderFactory);
        }

        private void GenerateAGraph(object sender, RoutedEventArgs e)
        {
            LoadingOn();
            CreateAllLayersOfGraph(_mapFactory, _randomizer, _pathfinderFactory);
            VisualMap.InitilizeLogicCore(_mapAdapter.VisualMap);
            VisualMap.InitilizeVisuals();
            InitializeEventHandlers();
            VisualMap.InitializeEventHandlers();
        }

        private void SetStartingPoint(object sender, RoutedEventArgs e)
        {
            VertexControl vertex = ((ItemsControl) sender).GetVertexControl();
            NodeView node = vertex.GetNodeView();

            if (_validator.ValidateCanBeDefinedPosition(vertex))
            {
                _mapAdapter.SetAsStartingPoint(node);
                VisualMap.SetStartingNode(vertex);
            }
        }

        private void SetTargetPoint(object sender, RoutedEventArgs e)
        {
            VertexControl vertex = ((ItemsControl)sender).GetVertexControl();
            NodeView node = vertex.GetNodeView();

            if (_validator.ValidateCanBeDefinedPosition(vertex))
            {
                _mapAdapter.SetAsTargetPoint(node);
                VisualMap.SetTargetNode(vertex);
            }
        }

        private void DeleteNode(object sender, RoutedEventArgs e)
        {
            VertexControl vertex = ((ItemsControl)sender).GetVertexControl();
            NodeView node = vertex.GetNodeView();

            VisualMap.RemoveVertexAndEdges(node, EdgesType.All, false, false);
            _mapAdapter.DeleteNode(node);
        }

        private void DisplayAlgorithmMonitor()
        {
            AlgorithmStats.Visibility = Visibility.Visible;
            StepsTaken.Content = Monitor.PathDetails.StepsTaken;
        }

        private void ChangeCost(object sender, RoutedEventArgs e)
        {
            EdgeControl edgeControl = ((ItemsControl)sender).GetEdgeControl();
            EdgeView edge = edgeControl.GetEdgeView();

            ChangeEdgeCostModal modal = new ChangeEdgeCostModal($"{edge.Caption} {Application.Current.Resources["ChangeEdgeCost"]}", edge.LogicEdge.Cost);
            bool? answered = modal.ShowDialog();

            if (answered != null && answered.Value == true)
                _mapAdapter.ChangeCost(edge, modal.Answer);
        }

        private void DeleteEdge(object sender, RoutedEventArgs eventArgs)
        {
            EdgeControl edgeControl = ((ItemsControl)sender).GetEdgeControl();
            EdgeView edge = edgeControl.GetEdgeView();
            EdgeView symetricEdge = _mapAdapter.VisualMap.Edges.Single(e => e != edge && e.Target.LogicNode == edge.Source.LogicNode && e.Source.LogicNode == edge.Target.LogicNode);

            VisualMap.RemoveEdge(edge);
            VisualMap.RemoveEdge(symetricEdge);
            _mapAdapter.DeleteEdge(edge);
            _mapAdapter.DeleteEdge(symetricEdge);
        }

        private void TakePathfindingStepCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Command.CanExecute(_mapAdapter);
        }

        private void TakePathfindingStepExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                e.Command.Execute((ePathfindingAlgorithms)ChoosenAlgorithm.SelectedIndex);
            }
            catch (DomainException exception)
            {
                _errorDisplayer.DisplayError(eErrorTypes.PathfindingError, exception.Message);
            }
            catch (Exception exception)
            {
                _errorDisplayer.DisplayError(eErrorTypes.General, exception.Message);
            }
            DisplayAlgorithmMonitor();
        }
    }
}