using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace MagisterkaTests
{
    public static class TestCommon
    {
        public static IEnumerable<Position> GenerateListOfCoordinates(Position startingPosition, Position endingPosition, int? numberOfNodes = null)
        {
            numberOfNodes = numberOfNodes ?? 20;

            yield return startingPosition;

            for (int i = 0; i < numberOfNodes.Value - 2; i++)
            {
                yield return new Position(Guid.NewGuid());
            }

            yield return endingPosition;
        }
    }
}
