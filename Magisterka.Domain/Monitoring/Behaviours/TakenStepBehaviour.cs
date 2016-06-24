using Magisterka.Domain.Monitoring.Quality;

namespace Magisterka.Domain.Monitoring.Behaviours
{
    public class TakenStepBehaviour : IAlgorithmBehaviour<PathDetails>
    {
        public void Register(PathDetails pathDetails)
        {
            ++pathDetails.StepsTaken;
        }
    }
}
