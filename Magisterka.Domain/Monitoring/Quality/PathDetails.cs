using System.Collections.Generic;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Monitoring.Quality
{
    public class PathDetails : IMonitorResults
    {
        public int StepsTaken { get; set; }
        public Dictionary<Node, int> NumberOfVisitsPerNode { get; set; }
        public long PathLengthInEdgeCost { get; set; }
    }
}
