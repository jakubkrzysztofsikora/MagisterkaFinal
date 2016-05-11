using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Domain.Graph.MovementSpace.Exceptions
{
    public class NodeNotOnTheGridException : Exception
    {
        public NodeNotOnTheGridException() : base("Node does not have position defined. It is not on the graph grid.")
        {
            
        }
    }
}
