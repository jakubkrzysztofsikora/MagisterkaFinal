using System.Collections.Generic;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Monitoring.Quality;

namespace Magisterka.Domain.Monitoring.Behaviours
{
    public class NodeVisitedBehaviour : IAlgorithmBehaviour<PathDetails>
    {
        private readonly Node _currentNode;

        public NodeVisitedBehaviour(Node currentNode)
        {
            _currentNode = currentNode;
        }

        public void Register(PathDetails pathDetails)
        {
            IntroduceNodeIntoRegistryIfNeeded(pathDetails.NumberOfVisitsPerNode);
            ++pathDetails.NumberOfVisitsPerNode[_currentNode.Coordinates];
        }

        private void IntroduceNodeIntoRegistryIfNeeded(Dictionary<Position, int> registry)
        {
            if (!registry.ContainsKey(_currentNode.Coordinates))
            {
                registry[_currentNode.Coordinates] = 0;
            }
        }
    }
}
