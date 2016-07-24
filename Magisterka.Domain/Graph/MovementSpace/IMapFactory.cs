using System.Collections.Generic;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Graph.MovementSpace
{
    public interface IMapFactory
    {
        Map GenerateDefaultMap();
        Map GenerateMapWithProvidedCoordinates(IEnumerable<Position> coordinates);
    }
}
