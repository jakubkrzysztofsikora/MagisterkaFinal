using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Domain.Graph.Pathfinding.Exceptions
{
    public class NegativeWeightCycleException : Exception
    {
        public NegativeWeightCycleException() : base("Graph contains a negative-weight cycle.")
        {
            
        }
    }
}
