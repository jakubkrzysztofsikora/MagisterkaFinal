using System.Collections.Generic;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;

namespace Magisterka.Domain.Monitoring
{
    public class PathDetails
    {
        public int StepsTaken { get; set; }
        public Dictionary<Position, int> NumberOfVisitsPerNode { get; set; }
        public long PathLengthInEdgeCost { get; set; }
    }
}
