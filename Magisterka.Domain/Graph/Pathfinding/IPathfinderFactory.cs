using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Domain.Graph.Pathfinding
{
    public interface IPathfinderFactory
    {
        Pathfinder CreatePathfinderWithAlgorithm(ePathfindingAlgorithms algorithm);
    }
}
