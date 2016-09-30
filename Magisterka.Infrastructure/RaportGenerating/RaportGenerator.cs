using System.IO;
using Magisterka.Domain.AppSettings;

namespace Magisterka.Infrastructure.RaportGenerating
{
    public class RaportGenerator : IRaportGenerator
    {
        private readonly IAppSettings _appSettings;

        public RaportGenerator(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public string GenerateRaport(IRaportCommand command)
        {
            if (!Directory.Exists(_appSettings.RaportPath))
                Directory.CreateDirectory(_appSettings.RaportPath);

            return command.CreateRaportFile(_appSettings.RaportPath);
        }
    }
}
