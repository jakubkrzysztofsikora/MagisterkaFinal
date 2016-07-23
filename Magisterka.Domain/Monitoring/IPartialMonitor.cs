using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Domain.Monitoring
{
    public interface IPartialMonitor<out TResults> 
        where TResults : IMonitorResults
    {
        void Start();
        TResults Stop();
    }
}
