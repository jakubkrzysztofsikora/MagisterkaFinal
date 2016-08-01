using Magisterka.Domain.Monitoring;

namespace Magisterka.ViewModels
{
    public class MainWindowViewModel
    {
        public IAlgorithmMonitor AlgorithmMonitor { get; set; }

        public MainWindowViewModel(IAlgorithmMonitor algorithmMonitor)
        {
            AlgorithmMonitor = algorithmMonitor;
        }
    }
}
