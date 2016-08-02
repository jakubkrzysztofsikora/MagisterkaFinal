using System;

namespace Magisterka.Domain.Utilities
{
    public class DefaultRandomGenerator : IRandomGenerator
    {
        private readonly Random _randomizer;

        public DefaultRandomGenerator()
        {
            _randomizer = new Random();
        }

        public bool GenerateNodeBlockedStatus()
        {
            return _randomizer.Next(0, 3) == 0;
        }

        public int GenerateNumberOfNeighbors(int minimum, int maximum)
        {
            return _randomizer.Next(minimum, maximum);
        }

        public int GenerateEdgeCost(int minimum, int maximum)
        {
            return _randomizer.Next(minimum, maximum);
        }
    }
}
