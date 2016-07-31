using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphX.Controls;
using GraphX.PCL.Common.Enums;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.ExceptionContracts;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.Animation;
using Magisterka.VisualEcosystem.ErrorHandling;
using Magisterka.VisualEcosystem.EventHandlers;
using Magisterka.VisualEcosystem.Extensions;
using Magisterka.VisualEcosystem.InputModals;
using Magisterka.VisualEcosystem.Validators;
using Magisterka.VisualEcosystem.WindowCommands;
using MahApps.Metro.Controls;
using FontAwesome = FontAwesome.WPF.FontAwesome;

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
        private TileMenuEventHandler _tileMenuEventHandler;
        private VisualMapEventHandler _visualMapEventHandler;
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

        private void InitializeEventHandlers()
        {
            NodeEventHandler.NameOfNodeContextMenu = "NodeContextMenu";
            NodeEventHandler.NameOfSetAsBlocked = "SetAsBlocked";
            NodeEventHandler.NameOfSetAsUnblocked = "SetAsUnblocked";

            VisualMap.VertexMouseEnter += NodeEventHandler.OnNodeHoverIn;
            VisualMap.VertexMouseLeave += NodeEventHandler.OnNodeHoverOut;
            VisualMap.VertexRightClick += NodeEventHandler.OnNodeRightClick;

            VisualMap.EdgeMouseEnter += EdgeEventHandler.OnEdgeHoverIn;
            VisualMap.EdgeMouseLeave += EdgeEventHandler.OnEdgeHoverOut;
            VisualMap.EdgeRightClick += EdgeEventHandler.OnEdgeRightClick;

            VisualMap.GenerateGraphFinished += (eAnimationSpeed, args) => LoadingOff();

            NewNodeTile.Click += _tileMenuEventHandler.ClickOnCreateANodeTile;
            NewEdgeTile.Click += _tileMenuEventHandler.ClickOnCreateAnEdgeTile;
            SizeChanged += (e,args) => ZoomControl.ZoomToFill();
        }

        private void UnsuscribeFromEvents()
        {
            VisualMap.VertexMouseEnter -= NodeEventHandler.OnNodeHoverIn;
            VisualMap.VertexMouseLeave -= NodeEventHandler.OnNodeHoverOut;
            VisualMap.VertexRightClick -= NodeEventHandler.OnNodeRightClick;

            VisualMap.EdgeMouseEnter -= EdgeEventHandler.OnEdgeHoverIn;
            VisualMap.EdgeMouseLeave -= EdgeEventHandler.OnEdgeHoverOut;
            VisualMap.EdgeRightClick -= EdgeEventHandler.OnEdgeRightClick;

            if (_tileMenuEventHandler != null)
            {
                NewNodeTile.Click -= _tileMenuEventHandler.ClickOnCreateANodeTile;
                NewEdgeTile.Click -= _tileMenuEventHandler.ClickOnCreateAnEdgeTile;
            }
        }

        private void LoadingOn()
        {
            ZoomControl.Visibility = Visibility.Hidden;
            GraphPlaceholder.Visibility = Visibility.Hidden;
            ProgressRing.Visibility = Visibility.Visible;
            ProgressRing.IsActive = true;
        }

        private void LoadingOff()
        {
            ZoomControl.Visibility = Visibility.Visible;
            ProgressRing.Visibility = Visibility.Hidden;
            ProgressRing.IsActive = false;
            ZoomControl.ZoomToFill();
        }

        private void CreateAllLayersOfGraph(IMapFactory mapFactory, Random randomizer, IPathfinderFactory pathfinderFactory, bool shouldGraphBeEmpty = false)
        {
            var map = (shouldGraphBeEmpty ? mapFactory.GenerateMap(0, 5) : mapFactory.GenerateDefaultMap())
                .WithGridPositions()
                .WithRandomBlockedNodes(randomizer);
            _mapAdapter = MapAdapter.CreateMapAdapterFromLogicMap(map, pathfinderFactory, mapFactory);
            _tileMenuEventHandler = new TileMenuEventHandler(_mapAdapter);
            _visualMapEventHandler = new VisualMapEventHandler(_mapAdapter, _validator, VisualMap);
        }

        private void GenerateAGraph(object sender, RoutedEventArgs e)
        {
            LoadingOn();
            RemoveAnyExistingGraphElements();
            UnsuscribeFromEvents();
            CreateAllLayersOfGraph(_mapFactory, _randomizer, _pathfinderFactory);
            VisualMap.InitilizeLogicCore(_mapAdapter.VisualMap);
            VisualMap.InitilizeVisuals();
            InitializeEventHandlers();
            VisualMap.InitializeEventHandlers();
        }

        private void RemoveAnyExistingGraphElements()
        {
            _mapAdapter?.DeleteGraphData();
            VisualMap.RemoveAllVertices();
            VisualMap.RemoveAllEdges();
        }

        private void SetStartingPoint(object sender, RoutedEventArgs e)
        {
            _visualMapEventHandler.SetStartingPoint(sender, e);
        }

        private void SetTargetPoint(object sender, RoutedEventArgs e)
        {
            _visualMapEventHandler.SetTargetPoint(sender, e);
        }

        private void DeleteNode(object sender, RoutedEventArgs e)
        {
            _visualMapEventHandler.DeleteNode(sender, e);
        }

        private void DisplayAlgorithmMonitor()
        {
            if (Monitor?.PathDetails != null && Monitor.PerformanceResults != null)
            {
                AlgorithmStats.Visibility = Visibility.Visible;
                StepsTaken.Content = Monitor.PathDetails.StepsTaken;
            }
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
            _visualMapEventHandler.DeleteEdge(sender, eventArgs);
        }

        private void CustomCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Command.CanExecute(_mapAdapter);
        }

        private void CustomCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                e.Command.Execute(new object[]
                {
                    (ePathfindingAlgorithms) ChoosenAlgorithm.SelectedValue,
                    (eAnimationSpeed) ChoosenAnimationSpeed.SelectedValue
                });
            }
            catch (DomainException exception)
            {
                _errorDisplayer.DisplayError(exception.ErrorType, exception.Message);
            }
            catch (Exception exception)
            {
                _errorDisplayer.DisplayError(eErrorTypes.General, exception.Message);
            }
            finally
            {
                DisplayAlgorithmMonitor();
            }
        }

        private void CreateNewGraph(object sender, ExecutedRoutedEventArgs e)
        {
            LoadingOn();
            RemoveAnyExistingGraphElements();
            UnsuscribeFromEvents();
            CreateAllLayersOfGraph(_mapFactory, _randomizer, _pathfinderFactory, true);
            VisualMap.InitilizeLogicCore(_mapAdapter.VisualMap);
            VisualMap.InitilizeVisuals();
            InitializeEventHandlers();
            VisualMap.InitializeEventHandlers();
        }

        private void SetBlockedNode(object sender, RoutedEventArgs e)
        {
            _visualMapEventHandler.SetBlockedNode(sender, e);
        }

        private void SetUnBlockedNode(object sender, RoutedEventArgs e)
        {
            _visualMapEventHandler.SetUnblockedNode(sender, e);
        }
    }
}