using System.Configuration;

namespace Votus.Core.Infrastructure.Data
{
    public class ProjectConfigFileRepository : IReadableRepository
    {
        public string Get(string settingName)
        {
            return ConfigurationManager.AppSettings[settingName];
        }
    }
}
