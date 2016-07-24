using System.Collections.Generic;

namespace Magisterka.Domain.Graph.MovementSpace.MapEcosystem
{
    public class Edge
    {
        public int Cost { get; set; }
        public KeyValuePair<Node, Node> NodesConnected { get; set; }
    }
}
