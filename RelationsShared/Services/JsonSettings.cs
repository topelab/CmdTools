using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace RelationsShared.Services
{
    public class JsonSettings : IJsonSettings
    {
        private JObject config;
        private string settingsFile;

        public JObject Config => config;

        public void Initialize(string settingsFile)
        {
            this.settingsFile = settingsFile;
            config = GetSettings(settingsFile);
        }

        public void ReloadSettings(params string[] additionalSettingFiles)
        {
            config = GetSettings(settingsFile);
            AddSettingFiles(additionalSettingFiles);
        }

        public void AddSettingFiles(params string[] additionalSettingFiles)
        {
            foreach (var additionalSettingFile in additionalSettingFiles)
            {
                var addedConfig = GetSettings(additionalSettingFile);
                config.Merge(addedConfig);
            }
        }

        private JObject GetSettings(string settingsFile)
        {
            JObject config;

            if (File.Exists(settingsFile))
            {
                var text = File.ReadAllText(settingsFile);
                config = (JObject)JsonConvert.DeserializeObject(text);
            }
            else
            {
                config = new JObject();
            }

            return config;
        }
    }
}
