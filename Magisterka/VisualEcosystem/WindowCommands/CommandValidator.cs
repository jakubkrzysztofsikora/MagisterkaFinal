using Magisterka.Domain.Adapters;
using Magisterka.Domain.ExceptionContracts;
using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.VisualEcosystem.Animation;

namespace Magisterka.VisualEcosystem.WindowCommands
{
    public class CommandValidator : ICommandValidator
    {
        public void ValidateConfiguration(MapAdapter mapAdapter, object[] enumParams)
        {
            if (mapAdapter == null || !(enumParams?[0] is ePathfindingAlgorithms) || !(enumParams[1] is eAnimationSpeed))
            {
                throw new DomainException("Bad graph configuration.")
                {
                    ErrorType = eErrorTypes.GraphConfiguration
                };
            }
        }
    }
}
