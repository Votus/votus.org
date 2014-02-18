using Ninject;
using System.Collections.Generic;
using System.Linq;
using Votus.Core.Infrastructure.Data;

namespace Votus.Core.Infrastructure.Configuration
{
    public class ConfigManager
    {
        private const string DefaultValueNotSpecified = null;

        [Inject]
        public List<IReadableRepository> Providers { get; set; }

        public virtual string Get(string settingName, string defaultValue)
        {
            foreach (var provider in Providers)
            {
                var value = provider.Get(settingName);

                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }

            if (defaultValue == DefaultValueNotSpecified)
                throw new SettingNotFoundException(settingName);

            return defaultValue;
        }

        public virtual string Get(string settingName)
        {
            return Get(settingName, defaultValue: null);
        }

        public void Set(string settingName, string value)
        {
            var valueWritten = false;
            var writableProviders = Providers.OfType<IWritableRepository>();

            foreach (var writableProvider in writableProviders)
            {
                writableProvider.Set(settingName, value);
                valueWritten = true;
            }

            if (valueWritten == false)
                throw new SaveSettingException(
                    "The setting value could not be saved because there are no writable repositories configured."
                );
        }
    }
}
