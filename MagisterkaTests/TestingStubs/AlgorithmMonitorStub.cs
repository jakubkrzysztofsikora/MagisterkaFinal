using System.ComponentModel;
using System.Runtime.CompilerServices;
using Magisterka.Domain.Annotations;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Monitoring;
using Magisterka.Domain.Monitoring.Performance;
using Magisterka.Domain.Monitoring.Quality;

namespace MagisterkaTests.TestingStubs
{
    public class AlgorithmMonitorStub : IAlgorithmMonitor
    {
        public PathDetails PathDetails { get; set; }
        public PerformanceResults PerformanceResults { get; set; }
        public bool IsMonitoring { get; set; }

        public void StartMonitoring()
        {
            
        }

        public void StopMonitoring()
        {
            
        }

        public void RecordStep()
        {
            
        }

        public void RecordNodeProcessed(params Node[] processedNodes)
        {
            
        }

        public void RecordEdgeCost(Node fromNode, Node toNode)
        {
            
        }

        public void MonitorPathFragment(Node fromNode, Node toNode)
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
