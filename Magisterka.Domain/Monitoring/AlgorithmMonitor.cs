using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GraphX.PCL.Logic.Helpers;
using Magisterka.Domain.Annotations;
using Magisterka.Domain.Graph.MovementSpace.MapEcosystem;
using Magisterka.Domain.Monitoring.Behaviours;
using Magisterka.Domain.Monitoring.Performance;
using Magisterka.Domain.Monitoring.Quality;

namespace Magisterka.Domain.Monitoring
{
    public class AlgorithmMonitor : IAlgorithmMonitor
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public PathDetails PathDetails { get; set; }
        public PerformanceResults PerformanceResults { get; set; }
        public bool IsMonitoring { get; set; }

        private readonly IPartialMonitor<PerformanceResults> _performanceMonitor;
        private readonly IBehaviourRegistry<PathDetails> _qualityRegistry;
        private const int MaximumNumberOfPerformanceReadsForCharts = 10;

        public AlgorithmMonitor(IPartialMonitor<PerformanceResults> performanceMonitor,
                                IBehaviourRegistry<PathDetails> qualityMonitor)
        {
            _performanceMonitor = performanceMonitor;
            _qualityRegistry = qualityMonitor;
        }

        public void StartMonitoring()
        {
            IsMonitoring = true;
            _qualityRegistry.StartRegistration();
            _performanceMonitor.Start();
        }

        public void StopMonitoring()
        {
            IsMonitoring = false;
            PerformanceResults = _performanceMonitor.Stop();
            PathDetails = _qualityRegistry.StopRegistration();
            PerformanceResults.MemoryUsageDictionary =
                SanatizePerformanceReadsForChartVisualization(PerformanceResults.MemoryUsageDictionary);
            PerformanceResults.ProcessorUsageDictionary =
                SanatizePerformanceReadsForChartVisualization(PerformanceResults.ProcessorUsageDictionary);


            OnPropertyChanged(nameof(PathDetails));
            OnPropertyChanged(nameof(PerformanceResults));
        }

        public void RecordStep()
        {
            TakenStepBehaviour behaviour = new TakenStepBehaviour();
            _qualityRegistry.NoteBehaviour(behaviour);
        }

        public void RecordNodeProcessed(params Node[] processedNodes)
        {
            processedNodes.ForEach(node =>
            {
                NodeProcessedBehaviour behaviour = new NodeProcessedBehaviour(node);
                _qualityRegistry.NoteBehaviour(behaviour);
            });
        }

        public void RecordEdgeCost(Node fromNode, Node toNode)
        {
            EdgeTraveledBehaviour behaviour = new EdgeTraveledBehaviour(fromNode, toNode);
            _qualityRegistry.NoteBehaviour(behaviour);
        }

        public void MonitorPathFragment(Node fromNode, Node toNode)
        {
            if (IsMonitoring)
            {
                RecordStep();
                RecordEdgeCost(fromNode, toNode);
            }
        }

        public void Clear()
        {
            PathDetails = new PathDetails();
            PerformanceResults = new PerformanceResults();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private IDictionary<TimeSpan, long> SanatizePerformanceReadsForChartVisualization(IDictionary<TimeSpan, long> performanceReads)
        {
            int numberOfReads = performanceReads.Count;
            int readInterval = numberOfReads / MaximumNumberOfPerformanceReadsForCharts;
            int countOfReads = 0;

            if (numberOfReads <= 10)
                return performanceReads;

            IDictionary<TimeSpan, long> sanatizedPerformanceReads = new Dictionary<TimeSpan, long>();

            performanceReads.ForEach(read =>
            {
                bool isReadChosenForChart = countOfReads%readInterval == 0 || countOfReads == 0 ||
                                            (performanceReads.Values.Max() == read.Value && !sanatizedPerformanceReads.Values.Contains(read.Value) );
                if (isReadChosenForChart)
                {
                    sanatizedPerformanceReads.Add(read);
                }
                ++countOfReads;
            });

            return sanatizedPerformanceReads;
        }
    }
}
