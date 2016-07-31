using System.Linq;
using Magisterka.Domain.Graph.MovementSpace.Exceptions;

namespace Magisterka.Domain.Graph.MovementSpace
{
    public class MapValidator
    {
        private readonly Map _map;

        public MapValidator(Map map)
        {
            _map = map;
        }

        public void ValidateNodes()
        {
            if (_map.Any(node => !node.Coordinates.X.HasValue || !node.Coordinates.Y.HasValue))
            {
                throw new NodeNotOnTheGridException();
            }
        }
    }
}
