using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Domain.Graph.MovementSpace.Exceptions
{
    public class DuplicatedPositionOnGridException : Exception
    {
        public DuplicatedPositionOnGridException() : base("There already is a node with that position.")
        {
            
        }
    }
}
