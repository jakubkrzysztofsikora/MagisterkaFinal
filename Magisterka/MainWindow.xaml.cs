﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GraphX.Controls;
using GraphX.Controls.Models;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using GraphX.PCL.Logic.Models;
using Magisterka.Domain;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem;
using Magisterka.VisualEcosystem.EventHandlers;
using Magisterka.VisualEcosystem.Extensions;
using Magisterka.VisualEcosystem.Validators;
using QuickGraph;

namespace Magisterka
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MapAdapter _mapAdapter;
        private ConfigurationValidator _validator;
        private ValidationErrorDisplayer _errorDisplayer;

        public MainWindow()
        {
            InitializeComponent();
            CreateAllLayersOfGraph();
            _errorDisplayer = new ValidationErrorDisplayer();
            _validator = new ConfigurationValidator(_errorDisplayer);

            VisualMap.InitilizeLogicCore(_mapAdapter.VisualMap);

            VisualMap.VertexMouseEnter += NodeEventHandler.OnNodeHoverIn;
            VisualMap.VertexMouseLeave += NodeEventHandler.OnNodeHoverOut;
            VisualMap.VertexRightClick += NodeEventHandler.OnNodeRightClick;

            VisualMap.EdgeMouseEnter += EdgeEventHandler.OnEdgeHoverIn;
            VisualMap.EdgeMouseLeave += EdgeEventHandler.OnEdgeHoverOut;

            VisualMap.InitilizeVisuals();

            ZoomControl.ZoomToFill();
        }

        public void Dispose()
        {
            VisualMap.Dispose();
        }

        private void CreateAllLayersOfGraph()
        {
            var mapFactory = new MapFactory(new Random());
            var map = mapFactory.GenerateDefaultMap()
                .WithGridPositions()
                .WithRandomBlockedNodes(new Random());
            _mapAdapter= MapAdapter.CreateMapAdapterFromLogicMap(map);
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
                VisualMap.RemoveStartLabel();
                VisualMap.CreateLabelForNode(node);
                VisualMap.SetCurrentNode(((ItemsControl)sender).GetVertexControl());
            }
        }

        private void SetTargetPoint(object sender, RoutedEventArgs e)
        {
            VertexControl vertex = ((ItemsControl)sender).GetVertexControl();
            NodeView node = vertex.GetNodeView();

            if (_validator.ValidateCanBeDefinedPosition(vertex))
            {
                _mapAdapter.SetAsTargetPoint(node);
                VisualMap.RemoveTargetLabel();
                VisualMap.CreateLabelForNode(node);
            }
        }

        private void StartPathfinding(object sender, RoutedEventArgs e)
        {
            VertexControl currentVertex = VisualMap.GetCurrentVertex();

            try
            {
                NodeView nextNode = _mapAdapter.StartPathfinding(currentVertex.GetNodeView(), ePathfindingAlgorithms.FloydWarshall);
                VertexControl nextVertexControl = VisualMap.GetVertexControlOfNode(nextNode);
                VisualMap.SetCurrentNode(nextVertexControl);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, Application.Current.Resources["UnknownPathfindingError"].ToString());
            }
        }
    }
}