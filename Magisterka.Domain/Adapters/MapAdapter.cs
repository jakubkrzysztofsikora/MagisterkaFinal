using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.PCL.Logic.Helpers;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.ViewModels;

namespace Magisterka.Domain.Adapters
{
    public class MapAdapter
    {
        public MapView VisualMap { get; set; }
        private readonly Map _logicMap;

        public MapAdapter(Map logicMap)
        {
            _logicMap = logicMap;
            VisualMap = new MapView();
            long nodeCounter = 0;
            _logicMap.Select(node => new NodeView
            {
                ID = nodeCounter++,
                LogicNode = node
            }).ForEach(nodeView =>
            {
                VisualMap.AddVertex(nodeView);
            });

            VisualMap.Vertices.Select( nodeView => nodeView.LogicNode).ForEach(node =>
            {
                IEnumerable<Edge> edges = node.Neighbors.Select(neighborToEdge => neighborToEdge.Value);
                long edgeCounter = 0;
                VisualMap.AddEdgeRange(
                    edges.Select(edge => new EdgeView(edge, VisualMap.GetVertexByLogicNode(edge.NodesConnected.Key),
                        VisualMap.GetVertexByLogicNode(edge.NodesConnected.Value))
                    {
                        ID = edgeCounter++
                    }));
            });
        }
    }
}
