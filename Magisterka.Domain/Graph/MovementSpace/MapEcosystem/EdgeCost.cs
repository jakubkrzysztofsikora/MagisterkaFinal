using System.Collections.Generic;

namespace Magisterka.Domain.Graph.MovementSpace.MapEcosystem
{
    public class EdgeCost
    {
        public int Value { get; set; }
        public KeyValuePair<Node, Node> NodesConnected { get; set; }
    }
}
