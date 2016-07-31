using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void RecordVisit(Node currentNode)
        {
            
        }

        public void RecordEdgeCost(Node fromNode, Node toNode)
        {
            
        }

        public void MonitorPathFragment(Node fromNode, Node toNode)
        {
            
        }
    }
}
