using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Utilities;
using Magisterka.Domain.ViewModels;
using Magisterka.ViewModels;
using Magisterka.VisualEcosystem.EventHandlers;
using Magisterka.VisualEcosystem.Extensions;
using Magisterka.VisualEcosystem.InputModals;
using Magisterka.VisualEcosystem.Validators;
using MahApps.Metro.Controls;

namespace Magisterka
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly IRandomGenerator _randomizer;
        private readonly IConfigurationValidator _validator;
        private readonly IMapFactory _mapFactory;
        private readonly IPathfinderFactory _pathfinderFactory;

        private readonly MainWindowViewModel _viewModel;

        public MainWindow(IConfigurationValidator validator,
                          IMapFactory mapFactory,
                          IPathfinderFactory pathfinderFactory,
                          IRandomGenerator randomizer,
                          MainWindowViewModel viewModel)
        {
            _validator = validator;
            _mapFactory = mapFactory;
            _pathfinderFactory = pathfinderFactory;
            _randomizer = randomizer;
            _viewModel = viewModel;
            
            InitializeComponent();
            _viewModel.VisualMap = VisualMap;
            _viewModel.ZoomControl = ZoomControl;
            
            DataContext = _viewModel;
            _viewModel.VisualMap.ShowEdgeArrows = false;
            _viewModel.VisualMap.ShowEdgeLabels = false;
            _viewModel.VisualMap.ShowVerticlesLabels = false;
            _viewModel.VisualMap.VerticlesDragging = true;
            _viewModel.SetDefaultIcons();
        }

        private void InitializeEventHandlers()
        {
            _viewModel.InitializeEventHandlers();
            NewNodeTile.Click += _viewModel.TileMenuEventHandler.ClickOnCreateANodeTile;
            NewEdgeTile.Click += _viewModel.TileMenuEventHandler.ClickOnCreateAnEdgeTile;
            SizeChanged += (e,args) => _viewModel.ZoomControl.ZoomToFill();
        }

        private void UnsuscribeFromEvents()
        {
            _viewModel.UnsuscribeFromEvents();
            if (_viewModel.TileMenuEventHandler != null)
            {
                NewNodeTile.Click -= _viewModel.TileMenuEventHandler.ClickOnCreateANodeTile;
                NewEdgeTile.Click -= _viewModel.TileMenuEventHandler.ClickOnCreateAnEdgeTile;
            }
        }

        private void CreateAllLayersOfGraph(IMapFactory mapFactory, IRandomGenerator randomizer, IPathfinderFactory pathfinderFactory, bool shouldGraphBeEmpty = false)
        {
            var map = (shouldGraphBeEmpty ? mapFactory.GenerateMap(0, 5) : mapFactory.GenerateDefaultMap())
                .WithGridPositions()
                .WithRandomBlockedNodes(randomizer);
            _viewModel.MapAdapter = MapAdapter.CreateMapAdapterFromLogicMap(map, pathfinderFactory, mapFactory);
            _viewModel.TileMenuEventHandler = new TileMenuEventHandler(_viewModel.MapAdapter)
            {
                MainWindowViewModel = _viewModel
            };
            _viewModel.VisualMapEventHandler = new VisualMapEventHandler(_viewModel.MapAdapter, _validator, _viewModel.VisualMap);
        }

        private void GenerateAGraph(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadingOn();
            _viewModel.RemoveAnyExistingGraphElements();
            UnsuscribeFromEvents();
            CreateAllLayersOfGraph(_mapFactory, _randomizer, _pathfinderFactory);
            _viewModel.VisualMap.InitilizeLogicCore(_viewModel.MapAdapter.VisualMap);
            _viewModel.VisualMap.InitilizeVisuals();
            InitializeEventHandlers();
        }

        private void SetStartingPoint(object sender, RoutedEventArgs e)
        {
            _viewModel.VisualMapEventHandler.SetStartingPoint(sender, e);
        }

        private void SetTargetPoint(object sender, RoutedEventArgs e)
        {
            _viewModel.VisualMapEventHandler.SetTargetPoint(sender, e);
        }

        private void DeleteNode(object sender, RoutedEventArgs e)
        {
            _viewModel.VisualMapEventHandler.DeleteNode(sender, e);
        }

        private void ChangeCost(object sender, RoutedEventArgs e)
        {
            EdgeControl edgeControl = ((ItemsControl)sender).GetEdgeControl();
            EdgeView edge = edgeControl.GetEdgeView();

            ChangeEdgeCostModal modal = new ChangeEdgeCostModal($"{edge.Caption} {Application.Current.Resources["ChangeEdgeCost"]}", edge.LogicEdge.Cost);
            bool? answered = modal.ShowDialog();

            if (answered != null && answered.Value == true)
                _viewModel.MapAdapter.ChangeCost(edge, modal.Answer);
        }

        private void DeleteEdge(object sender, RoutedEventArgs eventArgs)
        {
            _viewModel.VisualMapEventHandler.DeleteEdge(sender, eventArgs);
        }

        private void CreateNewGraph(object sender, ExecutedRoutedEventArgs e)
        {
            _viewModel.LoadingOn();
            _viewModel.RemoveAnyExistingGraphElements();
            UnsuscribeFromEvents();
            CreateAllLayersOfGraph(_mapFactory, _randomizer, _pathfinderFactory, true);
            _viewModel.VisualMap.InitilizeLogicCore(_viewModel.MapAdapter.VisualMap);
            _viewModel.VisualMap.InitilizeVisuals();
            InitializeEventHandlers();
        }

        private void SetBlockedNode(object sender, RoutedEventArgs e)
        {
            _viewModel.VisualMapEventHandler.SetBlockedNode(sender, e);
        }

        private void SetUnBlockedNode(object sender, RoutedEventArgs e)
        {
            _viewModel.VisualMapEventHandler.SetUnblockedNode(sender, e);
        }

        private void CustomCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            _viewModel.CustomCommandCanExecute(sender, e);
        }

        private void CustomCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _viewModel.CustomCommandExecuted(sender, e);
        }
    }
}