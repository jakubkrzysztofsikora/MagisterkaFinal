using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Graph.MovementSpace
{
    public interface IMapFactory
    {
        Map GenerateDefaultMap();
        Map GenerateMapWithProvidedCoordinates(IEnumerable<Position> coordinates);
    }
}
