using System;
using System.Windows;
using System.Windows.Controls;
using GraphX.Controls;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.ExceptionContracts;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.Animation;
using Magisterka.VisualEcosystem.Animation.AnimationCommands;
using Magisterka.VisualEcosystem.ErrorHandling;
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
        private readonly IMovingActor _actor;
        private readonly IErrorDisplayer _errorDisplayer;
        private readonly IConfigurationValidator _validator;
        private MapAdapter _mapAdapter;

        public MainWindow(IErrorDisplayer errorDisplayer, 
                          IConfigurationValidator validator,
                          IMapFactory mapFactory,
                          IPathfinderFactory pathfinderFactory,
                          IMovingActor actor,
                          Random randomizer)
        {
            _errorDisplayer = errorDisplayer;
            _validator = validator;
            _actor = actor;

            InitializeComponent();
            LoadingOn();
            CreateAllLayersOfGraph(mapFactory, randomizer, pathfinderFactory);
            VisualMap.InitilizeLogicCore(_mapAdapter.VisualMap);
            
            VisualMap.InitilizeVisuals();
            InitializeEventHandlers();
            VisualMap.InitializeEventHandlers();
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
            GraphTabs.Visibility = Visibility.Hidden;
            ProgressRing.IsActive = true;
        }

        private void LoadingOff()
        {
            GraphTabs.Visibility = Visibility.Visible;
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            VisualMap.RelayoutGraph(true);
            ZoomControl.ZoomToFill();
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

            _mapAdapter.DeleteNode(node);
        }

        private void StartPathfinding(object sender, RoutedEventArgs e)
        {
            VertexControl currentVertex = VisualMap.GetCurrentVertex();

            try
            {
                NodeView nextNode = _mapAdapter.StartPathfinding(currentVertex.GetNodeView(),
                    ePathfindingAlgorithms.FloydWarshall);
                VertexControl nextVertexControl = VisualMap.GetVertexControlOfNode(nextNode);

                var animation = new PathAnimationCommand(_actor, eAnimationSpeed.Fast)
                {
                    FromVertex = currentVertex,
                    ToVertex = nextVertexControl,
                    VisualMap = VisualMap
                };

                VisualMap.GoToVertex(nextVertexControl, animation);
            }
            catch (DomainException exception)
            {
                _errorDisplayer.DisplayError(eErrorTypes.PathfindingError, exception.Message);
            }
            catch (Exception exception)
            {
                _errorDisplayer.DisplayError(eErrorTypes.General, exception.Message);
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

        private void DeleteEdge(object sender, RoutedEventArgs e)
        {
            EdgeControl edgeControl = ((ItemsControl)sender).GetEdgeControl();
            EdgeView edge = edgeControl.GetEdgeView();

            _mapAdapter.DeleteEdge(edge);
        }
    }
}