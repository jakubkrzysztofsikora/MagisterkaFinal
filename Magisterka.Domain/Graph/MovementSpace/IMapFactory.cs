using System.Collections.Generic;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Graph.MovementSpace
{
    public interface IMapFactory
    {
        Map GenerateDefaultMap();
        Map GenerateMap(int numberOfNodes, int maxNumberOfNeighborsPerNode);
        Map GenerateMapWithProvidedCoordinates(IEnumerable<Position> coordinates);
        Node GenerateNewNode(int nodesCount);
    }
}
