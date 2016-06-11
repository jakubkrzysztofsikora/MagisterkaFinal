using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Domain.Graph.Pathfinding.Exceptions
{
    public class StartIsTargetPositionException : Exception
    {
        public StartIsTargetPositionException() : base("Starting node is also a target.")
        {
            
        }
    }
}
