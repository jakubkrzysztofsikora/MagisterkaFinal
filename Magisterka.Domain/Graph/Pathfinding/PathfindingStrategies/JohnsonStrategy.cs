﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.MovementSpace;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Graph.Pathfinding.PathfindingStrategies
{
    public class JohnsonStrategy : IPathfindingStrategy
    {
        public IEnumerable<Node> CalculatedPath { get; }
        public void Calculate(Map map, Position currentPosition)
        {
            throw new NotImplementedException();
        }
    }
}
