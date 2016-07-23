using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Magisterka.Infrastructure.RaportGenerating.RaportStaticResources;

namespace Magisterka.StaticResources
{
    public class RaportStringContainer : IRaportStringContainerContract
    {
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

        public RaportStringContainer()
        {
            RaportHeaderTitle = Application.Current.Resources["RaportHeaderTitle"].ToString();
            RaportHeaderDate = Application.Current.Resources["RaportHeaderDate"].ToString();
            RaportPathDetailsSectionTitle = Application.Current.Resources["RaportPathDetailsSectionTitle"].ToString();
            RaportNumberOfSteps = Application.Current.Resources["RaportNumberOfSteps"].ToString();
            RaportLengthOfPath = Application.Current.Resources["RaportLengthOfPath"].ToString();
            RaportVisitsPerNodeSectionTitle = Application.Current.Resources["RaportVisitsPerNodeSectionTitle"].ToString();
            RaportNodeName = Application.Current.Resources["RaportNodeName"].ToString();
            RaportNodeVisits = Application.Current.Resources["RaportNodeVisits"].ToString();
            RaportPerformanceResultsSectionTitle = Application.Current.Resources["RaportPerformanceResultsSectionTitle"].ToString();
            RaportTimeOfComputing = Application.Current.Resources["RaportTimeOfComputing"].ToString();
            RaportPeakMemoryUsage = Application.Current.Resources["RaportPeakMemoryUsage"].ToString();
            RaportAverageMemoryUsage = Application.Current.Resources["RaportAverageMemoryUsage"].ToString();
            RaportPeakProcessorUsage = Application.Current.Resources["RaportPeakProcessorUsage"].ToString();
            RaportAverageProcessorUsage = Application.Current.Resources["RaportAverageProcessorUsage"].ToString();
        }
    }
}
