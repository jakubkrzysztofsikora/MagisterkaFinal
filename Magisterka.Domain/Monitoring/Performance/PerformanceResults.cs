using System;
using System.Collections.Generic;

namespace Magisterka.Domain.Monitoring.Performance
{
    public class PerformanceResults : IMonitorResults
    {
        public TimeSpan TimeOfComputing { get; set; }
        public long PeakMemoryUsageInBytes { get; set; }
        public long AverageMemoryUsageInBytes { get; set; }
        public long PeakProcessorUsageInPercents { get; set; }
        public long AverageProcessorUsageInPercents { get; set; }

        public IDictionary<TimeSpan, long> MemoryUsageDictionary { get; set; }
        public IDictionary<TimeSpan, long> ProcessorUsageDictionary { get; set; }
    }
}
