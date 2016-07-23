using System.Configuration;

namespace Magisterka.Infrastructure.AppSettings
{
    public class AppSettings : IAppSettings
    {
        public AppSettings()
        {
            var configFile = ConfigurationManager.AppSettings;
            RaportPath = configFile["raportPath"] ?? "raports";
        }

        public string RaportPath { get; }
    }
}
