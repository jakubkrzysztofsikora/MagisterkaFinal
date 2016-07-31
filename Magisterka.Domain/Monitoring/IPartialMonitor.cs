namespace Magisterka.Domain.Monitoring
{
    public interface IPartialMonitor<out TResults> 
        where TResults : IMonitorResults
    {
        void Start();
        TResults Stop();
    }
}
