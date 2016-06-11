using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Domain.Graph.Pathfinding.Exceptions
{
    public class PathToTargetDoesntExistException : Exception
    {
        public PathToTargetDoesntExistException() : base("Path to target doesn't exist. Error in calculating the way.")
        {
            
        }
    }
}
