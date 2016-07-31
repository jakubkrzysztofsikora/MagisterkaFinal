using System;

namespace Magisterka.Domain.Graph.MovementSpace.Exceptions
{
    public class NodeNotOnTheGridException : Exception
    {
        public NodeNotOnTheGridException() : base("Node does not have position defined. It is not on the graph grid.")
        {
            
        }
    }
}
