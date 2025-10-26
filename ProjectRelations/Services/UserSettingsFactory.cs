namespace ProjectRelations.Services
{
    using System;
    using System.IO;

    internal class UserSettingsFactory(IJsonSettings jsonSetting) : IUserSettingsFactory
    {
        private readonly IJsonSettings jsonSetting = jsonSetting;

        public UserSettings Create()
        {
            var settingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "appsettings.json");
            EnsureSettingsFileExists(settingsFile);
            jsonSetting.Initialize(settingsFile);

            return jsonSetting.Config[nameof(UserSettings)].ToObject<UserSettings>() ?? new UserSettings();
        }

        private static void EnsureSettingsFileExists(string settingsFile)
        {
            if (!Directory.Exists(Path.GetDirectoryName(settingsFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(settingsFile)!);
            }
            if (!File.Exists(settingsFile))
            {
                var fileContent = $$"""
                    {
                        "{{nameof(UserSettings)}}": {
                        "{{nameof(UserSettings.CreateRelationsDiagramPath)}}": "C:\\PATH\\TO\\CreateRelationsDiagram.exe",
                        "{{nameof(UserSettings.ShowPackages)}}": true,
                        "{{nameof(UserSettings.Theme)}}": "Dark",
                        "{{nameof(UserSettings.BackgroundColor)}}": "black"
                      }
                    }
                    """;
                File.WriteAllText(settingsFile, fileContent);
            }
        }
    }
}
