namespace ProjectRelations2026.Services
{
    using Newtonsoft.Json.Linq;

    public interface IJsonSettings
    {
        JObject Config { get; }

        void AddSettingFiles(params string[] additionalSettingFiles);
        void ReloadSettings(params string[] additionalSettingFiles);
        void Initialize(string settingsFile);
    }
}