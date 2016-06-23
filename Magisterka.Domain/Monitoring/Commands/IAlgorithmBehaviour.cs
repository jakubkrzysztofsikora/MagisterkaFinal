namespace Magisterka.Domain.Monitoring.Commands
{
    public interface IAlgorithmBehaviour<in TResults>
        where TResults : IMonitorResults
    {
        void Register(TResults pathDetails);
    }
}
