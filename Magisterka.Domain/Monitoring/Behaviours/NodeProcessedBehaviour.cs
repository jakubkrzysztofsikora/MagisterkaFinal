using System.Collections.Generic;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Monitoring.Quality;

namespace Magisterka.Domain.Monitoring.Behaviours
{
    public class NodeProcessedBehaviour : IAlgorithmBehaviour<PathDetails>
    {
        private readonly Node _currentNode;

        public NodeProcessedBehaviour(Node currentNode)
        {
            _currentNode = currentNode;
        }

        public void Register(PathDetails pathDetails)
        {
            IntroduceNodeIntoRegistryIfNeeded(pathDetails.NumberOfVisitsPerNode);
            ++pathDetails.NumberOfVisitsPerNode[_currentNode];
        }

        private void IntroduceNodeIntoRegistryIfNeeded(Dictionary<Node, int> registry)
        {
            if (!registry.ContainsKey(_currentNode))
            {
                registry[_currentNode] = 0;
            }
        }
    }
}
