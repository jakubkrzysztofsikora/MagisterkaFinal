using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FontAwesome.WPF;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.Annotations;
using Magisterka.Domain.ExceptionContracts;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using Magisterka.Infrastructure.RaportGenerating;
using Magisterka.Infrastructure.RaportGenerating.RaportStaticResources;
using Magisterka.VisualEcosystem;
using Magisterka.VisualEcosystem.Animation;
using Magisterka.VisualEcosystem.ErrorHandling;
using Magisterka.VisualEcosystem.EventHandlers;
using Magisterka.VisualEcosystem.WindowCommands;
using MahApps.Metro.Controls.Dialogs;

namespace Magisterka.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IErrorDisplayer _errorDisplayer;

        public MainWindowViewModel(IAlgorithmMonitor algorithmMonitor, 
            IMovingActor actor, 
            IRaportGenerator raportGenerator,
            IRaportStringContainerContract raportStringContent, 
            IErrorDisplayer errorDisplayer,
            IDialogCoordinator dialogCoordinator)
        {
            Monitor = algorithmMonitor;
            Monitor.PropertyChanged += OnMonitorChanged;
            _errorDisplayer = errorDisplayer;
            TakePathfindingStepCommand = new TakePathfindingStepCommand(this, actor, new CommandValidator());
            StartPathfindingSimulationCommand = new StartPathfindingSimulationCommand(this, actor, new CommandValidator(), errorDisplayer);
            ClearGraphCommand = new ClearGraphCommand(this, actor);
            AddNewNodeCommand = new AddNewNodeCommand(this);
            AddNewEdgeCommand = new AddNewEdgeCommand(this);
            RelayoutGraphCommand = new RelayoutGraphCommand(this);
            ToggleNodeDraggingCommand = new ToggleNodeDraggingCommand(this);
            ToggleEdgeLabelsCommand = new ToggleEdgeLabelsCommand(this);
            ToggleEdgeArrowsCommand = new ToggleEdgeArrowsCommand(this);
            GenerateExcelRaportCommand = new GenerateExcelRaportCommand(algorithmMonitor, raportGenerator, raportStringContent, dialogCoordinator, this);

            ChosenAlgorithm = ePathfindingAlgorithms.Djikstra;
            ChosenAnimationSpeed = eAnimationSpeed.Normal;
            PathStatsPanelVisibility = Visibility.Collapsed;
            PathStatsPlaceholderVisibility = Visibility.Visible;
            PerformanceStatsPanelVisibility = Visibility.Collapsed;
            PerformanceStatsPlaceholderVisibility = Visibility.Visible;
            GraphPlaceholderVisibility = Visibility.Visible;
            ProgressRingVisibility = Visibility.Hidden;
            ProgressRingIsActive = true;
            NewEdgeTileIsEnabled = false;
            NewNodeTileIsEnabled = false;
            ChartMilisecondInterval = 1;
        }

        public static ICommand TakePathfindingStepCommand { get; set; }
        public static ICommand StartPathfindingSimulationCommand { get; set; }
        public static ICommand ClearGraphCommand { get; set; }
        public static ICommand AddNewNodeCommand { get; set; }
        public static ICommand AddNewEdgeCommand { get; set; }
        public static ICommand RelayoutGraphCommand { get; set; }
        public static ICommand ToggleNodeDraggingCommand { get; set; }
        public static ICommand ToggleEdgeLabelsCommand { get; set; }
        public static ICommand ToggleEdgeArrowsCommand { get; set; }
        public static ICommand GenerateExcelRaportCommand { get; set; }

        public IAlgorithmMonitor Monitor { get; set; }
        public Dictionary<double, double> MemoryUsageViewModel { get; set; }
        public Dictionary<double, int> ProcessorUsageViewModel { get; set; }
        public MapAdapter MapAdapter { get; set; }
        public VisualMap VisualMap { get; set; }
        public ZoomControl ZoomControl { get; set; }
        public TileMenuEventHandler TileMenuEventHandler { get; set; }
        public VisualMapEventHandler VisualMapEventHandler { get; set; }

        public ePathfindingAlgorithms ChosenAlgorithm { get; set; }
        public eAnimationSpeed ChosenAnimationSpeed { get; set; }

        public FontAwesomeIcon DraggingIcon { get; set; }
        public FontAwesomeIcon EdgeArrowsIcon { get; set; }
        public FontAwesomeIcon EdgeLabelsIcon { get; set; }

        public Visibility PathStatsPlaceholderVisibility { get; set; }
        public Visibility PathStatsPanelVisibility { get; set; }
        public Visibility PerformanceStatsPlaceholderVisibility { get; set; }
        public Visibility PerformanceStatsPanelVisibility { get; set; }
        public Visibility GraphPlaceholderVisibility { get; set; }
        public Visibility ProgressRingVisibility { get; set; }

        public bool ProgressRingIsActive { get; set; }
        public bool NewNodeTileIsEnabled { get; set; }
        public bool NewEdgeTileIsEnabled { get; set; }
        public int ChartMilisecondInterval { get; set; }
        public int PerformancePanelHeight { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler ClearedGraph;

        public void SetDefaultIcons()
        {
            DraggingIcon = VisualMap.VerticlesDragging ? FontAwesomeIcon.Unlock : FontAwesomeIcon.Lock;
            EdgeLabelsIcon = VisualMap.ShowEdgeLabels ? FontAwesomeIcon.Eye : FontAwesomeIcon.EyeSlash;
            EdgeArrowsIcon = VisualMap.ShowEdgeArrows ? FontAwesomeIcon.Exchange : FontAwesomeIcon.Minus;
            OnPropertyChanged(nameof(DraggingIcon));
            OnPropertyChanged(nameof(EdgeLabelsIcon));
            OnPropertyChanged(nameof(EdgeArrowsIcon));
        }

        public void DisplayAlgorithmMonitor()
        {
            PathStatsPanelVisibility = Visibility.Visible;
            PathStatsPlaceholderVisibility = Visibility.Collapsed;
            PerformanceStatsPanelVisibility = Visibility.Visible;
            PerformanceStatsPlaceholderVisibility = Visibility.Collapsed;
            OnPropertyChanged(nameof(PathStatsPanelVisibility));
            OnPropertyChanged(nameof(PathStatsPlaceholderVisibility));
            OnPropertyChanged(nameof(PerformanceStatsPanelVisibility));
            OnPropertyChanged(nameof(PerformanceStatsPlaceholderVisibility));
            OnPropertyChanged(nameof(Monitor));
            OnPropertyChanged(nameof(Monitor.PathDetails));
            OnPropertyChanged(nameof(Monitor.PerformanceResults));
        }

        public void CustomCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Command.CanExecute(MapAdapter);
        }

        public void CustomCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                e.Command.Execute(new object[]
                {
                    ChosenAlgorithm,
                    ChosenAnimationSpeed
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
        }

        public void OnMonitorChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            var performanceResultsAdapter = new PerformanceResultsAdapter(Monitor.PerformanceResults);
            MemoryUsageViewModel = performanceResultsAdapter.GetMemoryUsageForChart(ChartMilisecondInterval);
            ProcessorUsageViewModel = performanceResultsAdapter.GetProcessorUsageFotChart(ChartMilisecondInterval);

            OnPropertyChanged(nameof(MemoryUsageViewModel));
            OnPropertyChanged(nameof(ProcessorUsageViewModel));
        }

        public void OnWindowResize(object sender, SizeChangedEventArgs eventArgs)
        {
            PerformancePanelHeight = (int)eventArgs.NewSize.Height - 200;
            OnPropertyChanged(nameof(PerformancePanelHeight));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void InitializeEventHandlers()
        {
            NodeEventHandler.MainWindowViewModel = this;
            NodeEventHandler.NameOfNodeContextMenu = "NodeContextMenu";
            NodeEventHandler.NameOfSetAsBlocked = "SetAsBlocked";
            NodeEventHandler.NameOfSetAsUnblocked = "SetAsUnblocked";

            VisualMap.VertexMouseEnter += NodeEventHandler.OnNodeHoverIn;
            VisualMap.VertexMouseLeave += NodeEventHandler.OnNodeHoverOut;
            VisualMap.VertexRightClick += NodeEventHandler.OnNodeRightClick;
            VisualMap.VertexMouseUp += NodeEventHandler.OnNodeMouseDown;

            VisualMap.EdgeMouseEnter += EdgeEventHandler.OnEdgeHoverIn;
            VisualMap.EdgeMouseLeave += EdgeEventHandler.OnEdgeHoverOut;
            VisualMap.EdgeRightClick += EdgeEventHandler.OnEdgeRightClick;

            VisualMap.GenerateGraphFinished += (eAnimationSpeed, args) => LoadingOff();
            VisualMap.InitializeGraphElementsEventHandlers();
        }

        public void UnsuscribeFromEvents()
        {
            VisualMap.VertexMouseEnter -= NodeEventHandler.OnNodeHoverIn;
            VisualMap.VertexMouseLeave -= NodeEventHandler.OnNodeHoverOut;
            VisualMap.VertexRightClick -= NodeEventHandler.OnNodeRightClick;
            VisualMap.VertexMouseUp -= NodeEventHandler.OnNodeMouseDown;

            VisualMap.EdgeMouseEnter -= EdgeEventHandler.OnEdgeHoverIn;
            VisualMap.EdgeMouseLeave -= EdgeEventHandler.OnEdgeHoverOut;
            VisualMap.EdgeRightClick -= EdgeEventHandler.OnEdgeRightClick;
        }

        public void LoadingOn()
        {
            ZoomControl.Visibility = Visibility.Hidden;
            GraphPlaceholderVisibility = Visibility.Hidden;
            ProgressRingVisibility = Visibility.Visible;
            ProgressRingIsActive = true;
            OnPropertyChanged(nameof(GraphPlaceholderVisibility));
            OnPropertyChanged(nameof(ProgressRingVisibility));
            OnPropertyChanged(nameof(ProgressRingIsActive));
        }

        public void LoadingOff()
        {
            ZoomControl.Visibility = Visibility.Visible;
            ProgressRingVisibility = Visibility.Hidden;
            ProgressRingIsActive = false;
            ZoomControl.ZoomToFill();
            NewNodeTileIsEnabled = true;
            NewEdgeTileIsEnabled = true;
            OnPropertyChanged(nameof(NewNodeTileIsEnabled));
            OnPropertyChanged(nameof(NewEdgeTileIsEnabled));
            OnPropertyChanged(nameof(ProgressRingVisibility));
            OnPropertyChanged(nameof(ProgressRingIsActive));
        }

        public void RemoveAnyExistingGraphElements()
        {
            MapAdapter?.DeleteGraphData();
            VisualMap.RemoveAllVertices();
            VisualMap.RemoveAllEdges();
        }

        public void GraphCleared()
        {
            ClearedGraph?.Invoke(this, EventArgs.Empty);
        }
    }
}
