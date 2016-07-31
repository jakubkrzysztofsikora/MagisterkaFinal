using Magisterka.Domain.ExceptionContracts;

namespace Magisterka.Domain.Graph.Pathfinding.Exceptions
{
    public class PathToTargetDoesntExistException : DomainException
    {
        public PathToTargetDoesntExistException() : base("Path to target doesn't exist. Error in calculating the way.")
        {
            
        }
    }
}
