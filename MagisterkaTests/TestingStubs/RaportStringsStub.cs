using Magisterka.Infrastructure.RaportGenerating.RaportStaticResources;

namespace MagisterkaTests.TestingStubs
{
    public class RaportStringsStub : IRaportStringContainerContract
    {
        public RaportStringsStub()
        {
            RaportHeaderTitle = "Results";
            RaportHeaderDate = "Date of generation";
            RaportPathDetailsSectionTitle = "Path Details";
            RaportNumberOfSteps = "Number of steps taken";
            RaportLengthOfPath = "Physical length of the path";
            RaportVisitsPerNodeSectionTitle = "Visits per Node";
            RaportNodeName = "Node (Name)";
            RaportNodeVisits = "Number of visits";
            RaportPerformanceResultsSectionTitle = "Performance Results";
            RaportTimeOfComputing = "Time of computing (ms)";
            RaportPeakMemoryUsage = "Peak memory usage (MB)";
            RaportAverageMemoryUsage = "Average memory usage (MB)";
            RaportPeakProcessorUsage = "Peak processor usage (%)";
            RaportAverageProcessorUsage = "Average processor usage (%)";
        }

        public string RaportHeaderTitle { get; set; }
        public string RaportHeaderDate { get; set; }
        public string RaportPathDetailsSectionTitle { get; set; }
        public string RaportNumberOfSteps { get; set; }
        public string RaportLengthOfPath { get; set; }
        public string RaportVisitsPerNodeSectionTitle { get; set; }
        public string RaportNodeName { get; set; }
        public string RaportNodeVisits { get; set; }
        public string RaportPerformanceResultsSectionTitle { get; set; }
        public string RaportTimeOfComputing { get; set; }
        public string RaportPeakMemoryUsage { get; set; }
        public string RaportAverageMemoryUsage { get; set; }
        public string RaportPeakProcessorUsage { get; set; }
        public string RaportAverageProcessorUsage { get; set; }
    }
}
