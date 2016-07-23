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
        private long _memoryUsageOnStart;
        private List<long> _memoryUsageReads;
        private List<long> _cpuUsageReads;
        private PerformanceCounter _cpuCounter;
        private bool _stopMonitoring;

        private Task _monitoringOperation;

        public void Start()
        {
            InitMonitoringParameters();
            GatherBasePerformanceData();
            
            _monitoringOperation = Task.Run(() =>
            {
                do
                {
                    _memoryUsageReads.Add(GC.GetTotalMemory(WipeGarbageCollectorBeforeMonitor));
                    _cpuUsageReads.Add(Convert.ToInt64(_cpuCounter.NextValue()));
                } while (!_stopMonitoring);
            });
        }

        public PerformanceResults Stop()
        {
            StopMonitoringOperations();

            var performanceResults = GatherAllPerformanceMonitoringResults();

            return performanceResults;
        }

        private PerformanceResults GatherAllPerformanceMonitoringResults()
        {
            return new PerformanceResults
            {
                AverageMemoryUsageInBytes = Convert.ToInt64(_memoryUsageReads.Average()) - _memoryUsageOnStart,
                PeakMemoryUsageInBytes = _memoryUsageReads.Max() - _memoryUsageOnStart,
                AverageProcessorUsageInPercents = Convert.ToInt64(_cpuUsageReads.Average()),
                PeakProcessorUsageInPercents = Convert.ToInt64(_cpuUsageReads.Max()),
                TimeOfComputing = TimeSpan.FromTicks(DateTime.Now.Ticks - _amountOfTicksOnStart)
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
            _memoryUsageReads = new List<long>();
            _cpuUsageReads = new List<long>();
        }

        private void GatherBasePerformanceData()
        {
            _amountOfTicksOnStart = DateTime.Now.Ticks;
            _memoryUsageOnStart = GC.GetTotalMemory(WipeGarbageCollectorBeforeMonitor);
            _cpuCounter.NextValue();
        }
    }
}
