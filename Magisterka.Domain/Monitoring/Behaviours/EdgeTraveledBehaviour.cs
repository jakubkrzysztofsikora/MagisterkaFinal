using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Monitoring.Quality;

namespace Magisterka.Domain.Monitoring.Behaviours
{
    public class EdgeTraveledBehaviour : IAlgorithmBehaviour<PathDetails>
    {
        private readonly Node _fromNode;
        private readonly Node _toNode;

        public EdgeTraveledBehaviour(Node fromNode, Node toNode)
        {
            _fromNode = fromNode;
            _toNode = toNode;
        }

        public void Register(PathDetails pathDetails)
        {
            pathDetails.PathLengthInEdgeCost += _fromNode.Neighbors[_toNode].Cost;
        }
    }
}
