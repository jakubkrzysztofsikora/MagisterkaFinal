using Magisterka.Domain.ExceptionContracts;

namespace Magisterka.Domain.Graph.Pathfinding.Exceptions
{
    public class StartIsTargetPositionException : DomainException
    {
        public StartIsTargetPositionException() : base("Starting node is also a target.")
        {
            
        }
    }
}
