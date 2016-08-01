using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            OnPropertyChanged(nameof(PathDetails));
            OnPropertyChanged(nameof(PerformanceResults));
        }

        public void RecordStep()
        {
            TakenStepBehaviour behaviour = new TakenStepBehaviour();
            _qualityRegistry.NoteBehaviour(behaviour);
        }

        public void RecordVisit(Node currentNode)
        {
            NodeVisitedBehaviour behaviour = new NodeVisitedBehaviour(currentNode);
            _qualityRegistry.NoteBehaviour(behaviour);
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
                RecordVisit(fromNode);
                RecordEdgeCost(fromNode, toNode);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
