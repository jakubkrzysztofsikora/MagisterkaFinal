using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Adapters
{
    public class EdgeAdapter
    {
        public Edge Edge { get; set; }
        public Node FromNode { get; set; }
        public Node ToNode { get; set; }

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

            return mirroredEdgeAdapter;
        }
    }
}
