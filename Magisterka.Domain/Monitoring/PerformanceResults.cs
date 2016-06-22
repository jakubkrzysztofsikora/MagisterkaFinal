using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Domain.Monitoring
{
    public class PerformanceResults
    {
        public TimeSpan TimeOfComputing { get; set; }
        public long PeakMemoryUsageInBytes { get; set; }
        public long AverageMemoryUsageInBytes { get; set; }
        public long PeakProcessorUsageInPercents { get; set; }
        public long AverageProcessorUsageInPercents { get; set; }
    }
}
