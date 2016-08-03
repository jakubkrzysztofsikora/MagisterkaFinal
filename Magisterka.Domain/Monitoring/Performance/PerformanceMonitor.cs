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
        private long _memoryUsageOnStart;
        private Dictionary<TimeSpan, long> _memoryUsageReads;

        private Task _monitoringOperation;
        private bool _stopMonitoring;

        public void Start()
        {
            InitMonitoringParameters();
            GatherBasePerformanceData();
            
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
                registry.Add(TimeSpan.FromTicks(DateTime.Now.Ticks - _amountOfTicksOnStart), usage);
        }

        private PerformanceResults GatherAllPerformanceMonitoringResults()
        {
            long averageMemoryUsed = Math.Abs(Convert.ToInt64(_memoryUsageReads.Values.Average()) - _memoryUsageOnStart);
            return new PerformanceResults
            {
                AverageMemoryUsageInBytes = averageMemoryUsed,
                PeakMemoryUsageInBytes = _memoryUsageReads.Values.Max() - _memoryUsageOnStart,
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
            _memoryUsageOnStart = GC.GetTotalMemory(WipeGarbageCollectorBeforeMonitor);
            _cpuCounter.NextValue();
        }
    }
}
