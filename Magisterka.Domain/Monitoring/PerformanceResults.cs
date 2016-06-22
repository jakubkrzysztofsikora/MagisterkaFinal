using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Domain.Monitoring
{
    public class PerformanceResults
    {
        public long TimeOfComputing { get; set; }
        public int PeakMemoryUsageInBytes { get; set; }
        public int AverageMemoryUsageInBytes { get; set; }
        public int PeakProcessorUsageInPercents { get; set; }
        public int AverageProcessorUsageInPercents { get; set; }
    }
}
