using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Votus.Core.Infrastructure.IO;
using Votus.Core.Infrastructure.Serialization;

namespace Votus.Core.Infrastructure.Data
{
    public class LocalFileRepository : IReadWriteRepository
    {
        private readonly string _filePath;

        [Inject]
        public ISerializer Serializer { get; set; }

        public LocalFileRepository()
            : this(FindLocalConfigFile())
        {
        }

        public LocalFileRepository(string filePath)
        {
            _filePath = filePath;
        }

        public string Get(string settingName)
        {
            var settings = GetSettings();

            return settings.ContainsKey(settingName) ? 
                settings[settingName] : null;
        }

        public void Set(string settingName, string value)
        {
            var settings = GetSettings();
            settings[settingName] = value;
            SaveSettings(settings);
        }

        private void SaveSettings(Dictionary<string, string> settings)
        {
            var serializedSettings = Serializer.Serialize(settings);
            File.WriteAllText(_filePath, serializedSettings);
        }

        private Dictionary<string, string> GetSettings()
        {
            return Serializer.Deserialize<Dictionary<string, string>>(
                File.ReadAllText(_filePath)
            );
        }

        private static string FindLocalConfigFile()
        {
            const string settingsFileName = "settings.json";

            var startingPath = new FileInfo(
                new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath
            ).Directory;

            return startingPath.FindFileInParentTree(settingsFileName).FullName;
        }
    }
}
