using Magisterka.Domain.Monitoring.Commands;

namespace Magisterka.Domain.Monitoring.Quality
{
    public class AlgorithmQualityRegistry : IBehaviourRegistry<PathDetails>
    {
        private PathDetails _results;

        public void StartRegistration()
        {
            _results = new PathDetails();
        }

        public PathDetails StopRegistration()
        {
            return _results;
        }

        public void NoteBehaviour(IAlgorithmBehaviour<PathDetails> behaviour)
        {
            behaviour.Register(_results);
        }

        public PathDetails GetRegisteredResults()
        {
            return _results;
        }
    }
}
