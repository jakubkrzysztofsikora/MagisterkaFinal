using System.Collections.Generic;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Adapters
{
    public class EdgeAdapter
    {
        private Edge _edge;

        public Edge Edge
        {
            get
            {
                return _edge = _edge ?? new Edge
                {
                    Cost = 1,
                    NodesConnected = new KeyValuePair<Node, Node>(FromNode, ToNode)
                };
            }
            set
            {
                _edge = value;
            }
        }

        public Node FromNode { get; set; }
        public Node ToNode { get; set; }
        public MapAdapter MapAdapter { get; set; }

        public EdgeAdapter GetEdgeAdapterWithMirroredEdges()
        {
            EdgeAdapter mirroredEdgeAdapter = new EdgeAdapter();
            Node temporaryNode = new Node(FromNode.Neighbors)
            {
                Coordinates = FromNode.Coordinates,
                IsBlocked = FromNode.IsBlocked,
                IsStartingNode = FromNode.IsStartingNode,
                IsTargetNode = FromNode.IsTargetNode,
                Name = FromNode.Name
            };

            mirroredEdgeAdapter.FromNode = ToNode;
            mirroredEdgeAdapter.ToNode = temporaryNode;
            mirroredEdgeAdapter.Edge = Edge;
            mirroredEdgeAdapter.MapAdapter = MapAdapter;

            return mirroredEdgeAdapter;
        }
    }
}
