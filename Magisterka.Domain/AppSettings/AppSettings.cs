using System.Collections.Specialized;
using System.Configuration;

namespace Magisterka.Domain.AppSettings
{
    public class AppSettings : IAppSettings
    {
        public AppSettings()
        {
            var configFile = ConfigurationManager.AppSettings;
            RaportPath = configFile["raportPath"] ?? "raports";
            LoadRandomGraphConfig(configFile);
        }

        public string RaportPath { get; }
        public int RandomGraphDefaultNodeNumber { get; private set; }
        public int RandomGraphDefaultMaxNeighborsForNode { get; private set; }
        public int RandomGraphMinNeighborNumber { get; private set; }
        public int RandomGraphMinEdgeCost { get; private set; }
        public int RandomGraphMaxEdgeCost { get; private set; }

        private void LoadRandomGraphConfig(NameValueCollection configFile)
        {
            int defaultNodeNumber;
            int defaultMaxNeighborsForNode;
            int minNeighborsNumber;
            int minEdgeCost;
            int maxEdgeCost;

            int.TryParse(configFile["RandomGraphDefaultNodeNumber"], out defaultNodeNumber);
            int.TryParse(configFile["RandomGraphDefaultMaxNeighborsForNode"], out defaultMaxNeighborsForNode);
            int.TryParse(configFile["RandomGraphMinNeighborNumber"], out minNeighborsNumber);
            int.TryParse(configFile["RandomGraphMinEdgeCost"], out minEdgeCost);
            int.TryParse(configFile["RandomGraphMaxEdgeCost"], out maxEdgeCost);

            RandomGraphDefaultNodeNumber = defaultNodeNumber != 0 ? defaultNodeNumber : 20;
            RandomGraphDefaultMaxNeighborsForNode = defaultMaxNeighborsForNode != 0 ? defaultMaxNeighborsForNode : 4;
            RandomGraphMinNeighborNumber = minNeighborsNumber != 0 ? minNeighborsNumber : 1;
            RandomGraphMinEdgeCost = minEdgeCost != 0 ? minEdgeCost : 1;
            RandomGraphMaxEdgeCost = maxEdgeCost != 0 ? maxEdgeCost : 10;
        }
    }
}
