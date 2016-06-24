namespace Magisterka.Domain.Monitoring.Behaviours
{
    public interface IAlgorithmBehaviour<in TResults>
        where TResults : IMonitorResults
    {
        void Register(TResults pathDetails);
    }
}
