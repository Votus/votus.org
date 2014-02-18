using System.Configuration;
using Votus.Core.Infrastructure.Data;

namespace Votus.Testing.Unit.Core.Infrastructure.Data
{
    class ProjectConfigFileRepositoryTests : IReadableRepositoryTests
    {
        public ProjectConfigFileRepositoryTests()
            : base(new ProjectConfigFileRepository())
        {
        }

        protected override void SetValue(string key, string value)
        {
            ConfigurationManager.AppSettings[key] = value;
        }
    }
}
