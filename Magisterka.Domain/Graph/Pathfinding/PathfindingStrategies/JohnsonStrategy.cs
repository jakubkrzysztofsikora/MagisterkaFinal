using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Monitoring;

namespace Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies
{
    public class JohnsonStrategy : IPathfindingStrategy
    {
        public IEnumerable<Node> CalculatedPath { get; }

        private readonly IAlgorithmMonitor _monitor;

        public JohnsonStrategy(IAlgorithmMonitor monitor)
        {
            _monitor = monitor;
        }

        public void Calculate(Map map, Position currentPosition)
        {
            throw new NotImplementedException();
        }
    }
}
