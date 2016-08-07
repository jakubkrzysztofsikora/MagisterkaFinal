using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Magisterka.Domain.Monitoring.Performance
{
    public class PerformanceMonitor : IPartialMonitor<PerformanceResults>
    {
        private const bool WipeGarbageCollectorBeforeMonitor = false;

        private long _amountOfTicksOnStart;
        private PerformanceCounter _cpuCounter;
        private Dictionary<TimeSpan, long> _cpuUsageReads;
        private Dictionary<TimeSpan, long> _memoryUsageReads;

        private Task _monitoringOperation;
        private bool _stopMonitoring;

        public void Start()
        {
            InitMonitoringParameters();
            GatherBasePerformanceData();
            GC.GetTotalMemory(true);

            _monitoringOperation = Task.Run(() =>
            {
                do
                {
                    AddToUsageRegistry(_memoryUsageReads, GC.GetTotalMemory(WipeGarbageCollectorBeforeMonitor));
                    AddToUsageRegistry(_cpuUsageReads, Convert.ToInt64(_cpuCounter.NextValue()));
                } while (!_stopMonitoring);
            });
        }

        public PerformanceResults Stop()
        {
            StopMonitoringOperations();

            var performanceResults = GatherAllPerformanceMonitoringResults();

            return performanceResults;
        }

        private void AddToUsageRegistry(Dictionary<TimeSpan, long> registry, long usage)
        {
            var timeSpanKey = TimeSpan.FromTicks(DateTime.Now.Ticks - _amountOfTicksOnStart);
            usage = usage < 0 ? 0 : usage;
            if (!registry.ContainsKey(timeSpanKey))
                registry.Add(timeSpanKey, usage);
        }

        private PerformanceResults GatherAllPerformanceMonitoringResults()
        {
            return new PerformanceResults
            {
                AverageMemoryUsageInBytes = Convert.ToInt64(_memoryUsageReads.Values.Average()),
                PeakMemoryUsageInBytes = _memoryUsageReads.Values.Max(),
                AverageProcessorUsageInPercents = Convert.ToInt64(_cpuUsageReads.Values.Average()),
                PeakProcessorUsageInPercents = Convert.ToInt64(_cpuUsageReads.Values.Max()),
                TimeOfComputing = TimeSpan.FromTicks(DateTime.Now.Ticks - _amountOfTicksOnStart),
                MemoryUsageDictionary = _memoryUsageReads,
                ProcessorUsageDictionary = _cpuUsageReads
            };
        }

        private void StopMonitoringOperations()
        {
            _stopMonitoring = true;

            if (!_monitoringOperation.IsCompleted)
                _monitoringOperation.Wait();
        }

        private void InitMonitoringParameters()
        {
            _stopMonitoring = false;
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memoryUsageReads = new Dictionary<TimeSpan, long>();
            _cpuUsageReads = new Dictionary<TimeSpan, long>();
        }

        private void GatherBasePerformanceData()
        {
            _amountOfTicksOnStart = DateTime.Now.Ticks;
            _cpuCounter.NextValue();
        }
    }
}
