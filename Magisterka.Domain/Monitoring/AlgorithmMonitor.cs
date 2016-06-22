using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Magisterka.Domain.Monitoring
{
    public class AlgorithmMonitor
    {
        public PathDetails PathDetails { get; set; }
        public PerformanceResults PerformanceResults { get; set; }
        public bool IsMonitoring { get; set; }

        private long _amountOfTicksOnStart;
        private long _memoryUsageOnStart;
        private List<long> _memoryUsageReads;
        private List<long> _cpuUsageReads; 
        private CancellationTokenSource _performanceReadCancellationTokenSource;
        private PerformanceCounter _cpuCounter;

        public AlgorithmMonitor()
        {
            PathDetails = new PathDetails();
            PerformanceResults = new PerformanceResults();
        }

        public void StartMonitoring()
        {
            IsMonitoring = true;
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memoryUsageReads = new List<long>();
            _cpuUsageReads = new List<long>();
            _performanceReadCancellationTokenSource = new CancellationTokenSource();
            var token = _performanceReadCancellationTokenSource.Token;

            _amountOfTicksOnStart = DateTime.Now.Ticks;
            _memoryUsageOnStart = GC.GetTotalMemory(true);

            Task.Run(() =>
            {
                _cpuCounter.NextValue();
                while (true)
                {
                    _memoryUsageReads.Add(GC.GetTotalMemory(true));
                    _cpuUsageReads.Add(Convert.ToInt64(_cpuCounter.NextValue()));
                }
            }, token);
        }

        public void StopMonitoring()
        {
            IsMonitoring = false;
            _performanceReadCancellationTokenSource.Cancel();

            PerformanceResults.AverageMemoryUsageInBytes = Convert.ToInt64(_memoryUsageReads.Average()) - _memoryUsageOnStart;
            PerformanceResults.PeakMemoryUsageInBytes = _memoryUsageReads.Max() - _memoryUsageOnStart;

            PerformanceResults.AverageProcessorUsageInPercents = Convert.ToInt64(_cpuUsageReads.Any() ? _cpuUsageReads.Average() : _cpuCounter.NextValue());
            PerformanceResults.PeakProcessorUsageInPercents = Convert.ToInt64(_cpuUsageReads.Any() ? _cpuUsageReads.Max() : _cpuCounter.NextValue());

            PerformanceResults.TimeOfComputing = TimeSpan.FromTicks(DateTime.Now.Ticks - _amountOfTicksOnStart);
        }
    }
}
