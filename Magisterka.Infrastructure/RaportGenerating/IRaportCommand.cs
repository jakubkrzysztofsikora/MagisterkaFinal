using Magisterka.Domain.Graph.Pathfinding;
using Magisterka.Domain.Monitoring;
using Magisterka.Infrastructure.RaportGenerating.RaportStaticResources;

namespace Magisterka.Infrastructure.RaportGenerating
{
    public interface IRaportCommand
    {
        ePathfindingAlgorithms PathfindingAlgorithm { get; set; }
        IAlgorithmMonitor AlgorithmMonitor { get; set; }
        IRaportStringContainerContract RaportStrings { get; set; }
        string CreateRaportFile(string path);
    }
}
