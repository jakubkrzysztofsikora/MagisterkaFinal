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
using Magisterka.VisualEcosystem.Validators;

namespace Magisterka
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
            CreateAllLayersOfGraph(mapFactory, randomizer, pathfinderFactory);
            VisualMap.InitilizeLogicCore(_mapAdapter.VisualMap);
            InitializeEventHandlers();
            VisualMap.InitilizeVisuals();

            ZoomControl.ZoomToFill();
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

            _mapAdapter.Delete(node);
            VisualMap.UpdateLayout();
        }

        private void StartPathfinding(object sender, RoutedEventArgs e)
        {
            VertexControl currentVertex = VisualMap.GetCurrentVertex();

            try
            {
                NodeView nextNode = _mapAdapter.StartPathfinding(currentVertex.GetNodeView(),
                    ePathfindingAlgorithms.FloydWarshall);
                VertexControl nextVertexControl = VisualMap.GetVertexControlOfNode(nextNode);

                var animation = new PathAnimationCommand(_actor)
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
    }
}