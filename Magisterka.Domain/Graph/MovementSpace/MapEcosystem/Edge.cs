using System;
using System.Collections.Generic;
using GraphX.PCL.Common.Models;

namespace Magisterka.Domain.Graph.MovementSpace.MapEcosystem
{
    public class Edge
    {
        public int Cost { get; set; }
        public KeyValuePair<Node, Node> NodesConnected { get; set; }
    }
}
