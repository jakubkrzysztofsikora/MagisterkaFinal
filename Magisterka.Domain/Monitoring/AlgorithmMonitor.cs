namespace Magisterka.Domain.Monitoring
{
    public class AlgorithmMonitor
    {
        public PathDetails PathDetails { get; set; }
        public PerformanceResults PerformanceResults { get; set; }
        public bool IsMonitoring { get; set; }

        private readonly IPartialMonitor<PerformanceResults> _performanceMonitor;

        public AlgorithmMonitor(IPartialMonitor<PerformanceResults> performanceMonitor)
        {
            _performanceMonitor = performanceMonitor;
            PerformanceResults = new PerformanceResults();
        }

        public void StartMonitoring()
        {
            IsMonitoring = true;
            _performanceMonitor.Start();
        }

        public void StopMonitoring()
        {
            IsMonitoring = false;
            PerformanceResults = _performanceMonitor.Stop();
        }
    }
}
