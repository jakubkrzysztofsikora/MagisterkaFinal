using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GraphX.Controls;
using GraphX.PCL.Common.Enums;
using Magisterka.Domain.Adapters;
using Magisterka.Domain.ViewModels;
using Magisterka.VisualEcosystem.Extensions;
using Magisterka.VisualEcosystem.Validators;

namespace Magisterka.VisualEcosystem.EventHandlers
{
    public class VisualMapEventHandler
    {
        private readonly MapAdapter _mapAdapter;
        private readonly IConfigurationValidator _validator;
        private readonly VisualMap _visualMap;

        public VisualMapEventHandler(MapAdapter mapAdapter,
            IConfigurationValidator validator, 
            VisualMap visualMap)
        {
            _mapAdapter = mapAdapter;
            _validator = validator;
            _visualMap = visualMap;
        }

        public void SetStartingPoint(object sender, RoutedEventArgs e)
        {
            VertexControl vertex = ((ItemsControl)sender).GetVertexControl();
            NodeView node = vertex.GetNodeView();

            if (_validator.ValidateCanBeDefinedPosition(vertex))
            {
                _mapAdapter.SetAsStartingPoint(node);
                _visualMap.SetStartingNode(vertex);
            }
        }

        public void SetTargetPoint(object sender, RoutedEventArgs e)
        {
            VertexControl vertex = ((ItemsControl)sender).GetVertexControl();
            NodeView node = vertex.GetNodeView();

            if (_validator.ValidateCanBeDefinedPosition(vertex))
            {
                _mapAdapter.SetAsTargetPoint(node);
                _visualMap.SetTargetNode(vertex);
            }
        }

        public void DeleteNode(object sender, RoutedEventArgs e)
        {
            VertexControl vertex = ((ItemsControl)sender).GetVertexControl();
            NodeView node = vertex.GetNodeView();

            _visualMap.RemoveVertexAndEdges(node, EdgesType.All, false, false);
            _mapAdapter.DeleteNode(node);
        }

        public void DeleteEdge(object sender, RoutedEventArgs eventArgs)
        {
            EdgeControl edgeControl = ((ItemsControl)sender).GetEdgeControl();
            EdgeView edge = edgeControl.GetEdgeView();
            EdgeView symetricEdge = _mapAdapter.VisualMap.Edges.Single(e => e != edge && e.Target.LogicNode == edge.Source.LogicNode && e.Source.LogicNode == edge.Target.LogicNode);

            _visualMap.RemoveEdge(edge);
            _visualMap.RemoveEdge(symetricEdge);
            _mapAdapter.DeleteEdge(edge);
            _mapAdapter.DeleteEdge(symetricEdge);
        }

        public void SetBlockedNode(object sender, RoutedEventArgs e)
        {
            VertexControl vertex = ((ItemsControl)sender).GetVertexControl();
            NodeView node = vertex.GetNodeView();

            _mapAdapter.SetAsBlockedNode(node);
            _visualMap.MarkBlockedNode(vertex);
        }

        public void SetUnblockedNode(object sender, RoutedEventArgs routedEventArgs)
        {
            VertexControl vertex = ((ItemsControl)sender).GetVertexControl();
            NodeView node = vertex.GetNodeView();

            _mapAdapter.SetAsUnblocked(node);
            _visualMap.MarkUnblockedNode(vertex);
        }
    }
}
