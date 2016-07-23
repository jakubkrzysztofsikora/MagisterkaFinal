using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka.Infrastructure.RaportGenerating.RaportStaticResources
{
    public interface IRaportStringContainerContract
    {
        string RaportHeaderTitle { get; set; }
        string RaportHeaderDate { get; set; }
        string RaportPathDetailsSectionTitle { get; set; }
        string RaportNumberOfSteps { get; set; }
        string RaportLengthOfPath { get; set; }
        string RaportVisitsPerNodeSectionTitle { get; set; }
        string RaportNodeName { get; set; }
        string RaportNodeVisits { get; set; }
        string RaportPerformanceResultsSectionTitle { get; set; }
        string RaportTimeOfComputing { get; set; }
        string RaportPeakMemoryUsage { get; set; }
        string RaportAverageMemoryUsage { get; set; }
        string RaportPeakProcessorUsage { get; set; }
        string RaportAverageProcessorUsage { get; set; }
    }
}
