namespace Magisterka.Domain.Utilities
{
    public interface IRandomGenerator
    {
        bool GenerateNodeBlockedStatus();
        int GenerateNumberOfNeighbors(int minimum, int maximum);
        int GenerateEdgeCost(int minimum, int maximum);
    }
}
