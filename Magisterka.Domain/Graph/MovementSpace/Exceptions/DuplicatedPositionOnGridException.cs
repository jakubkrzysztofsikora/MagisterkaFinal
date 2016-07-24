using System;

namespace Magisterka.Domain.Graph.MovementSpace.Exceptions
{
    public class DuplicatedPositionOnGridException : Exception
    {
        public DuplicatedPositionOnGridException() : base("There already is a node with that position.")
        {
            
        }
    }
}
