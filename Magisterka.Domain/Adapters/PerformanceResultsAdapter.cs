using System;
using System.Collections.Generic;
using System.Linq;
using GraphX.PCL.Logic.Helpers;
using Magisterka.Domain.Converters;
using Magisterka.Domain.Monitoring.Performance;

namespace Magisterka.Domain.Adapters
{
    public class PerformanceResultsAdapter
    {
        private readonly PerformanceResults _performanceResults;

        public PerformanceResultsAdapter(PerformanceResults performanceResults)
        {
            _performanceResults = performanceResults;
        }

        public Dictionary<double, double> GetMemoryUsageForChart(int chartMilisecondInterval)
        {
            var timeOfReads = GetTimeOfReads(_performanceResults.MemoryUsageDictionary, chartMilisecondInterval);

            var memoryUsage = new Dictionary<double, double>();

            int indexOfTime = 0;
            timeOfReads.ForEach(time =>
            {
                memoryUsage.Add(time, _performanceResults.MemoryUsageDictionary.Where(x => x.Key.Milliseconds == time).Select(x => x.Value).Max().ToMegaBytes());
                ++indexOfTime;
            });

            return memoryUsage;
        }

        public Dictionary<double, int> GetProcessorUsageFotChart(int chartMilisecondInterval)
        {
            var timeOfReads = GetTimeOfReads(_performanceResults.ProcessorUsageDictionary, chartMilisecondInterval);

            var processorUsage = new Dictionary<double, int>();

            int indexOfTime = 0;
            timeOfReads.ForEach(time =>
            {
                processorUsage.Add(time, (int)_performanceResults.ProcessorUsageDictionary.Where(x => x.Key.Milliseconds == time).Select(x => x.Value).Max());
                ++indexOfTime;
            });

            return processorUsage;
        }

        private IEnumerable<int> GetTimeOfReads(IDictionary<TimeSpan, long> reads, int chartMilisecondInterval)
        {
            return reads.Keys.Select(x => x.Milliseconds)
                .Where(x => x%chartMilisecondInterval == 0)
                .Distinct();
        }
    }
}
