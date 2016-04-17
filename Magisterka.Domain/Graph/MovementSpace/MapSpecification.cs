using System.Linq;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Graph.MovementSpace
{
    public static class MapSpecification
    {
        public static Map WithStartingPosition(this Map map, Position startingPosition)
        {
            var startingNode = map.GetNodeByPosition(startingPosition);
            startingNode.IsStartingNode = true;

            return map;
        }

        public static Map WithTargetPosition(this Map map, Position endingPosition)
        {
            var endingNode = map.GetNodeByPosition(endingPosition);
            endingNode.IsTargetNode = true;

            return map;
        }

        public static Map WithNoDefinedPositions(this Map map)
        {
            foreach (var node in map.Where(node => node.IsStartingNode || node.IsTargetNode))
            {
                node.IsStartingNode = false;
                node.IsTargetNode = false;
            }

            return map;
        }
    }
}
