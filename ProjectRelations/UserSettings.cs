namespace ProjectRelations
{
    using Newtonsoft.Json;

    public class UserSettings
    {
        public string CreateRelationsDiagramPath { get; set; }
        public bool ShowPackages { get; set; } = true;
        public string Theme { get; set; } = "Dark";
        public string BackgroundColor { get; set; } = "black";

        [JsonIgnore]
        public bool HasTheme => !string.IsNullOrEmpty(Theme);
        [JsonIgnore]
        public bool HasBackgroundColor => !string.IsNullOrEmpty(BackgroundColor);
    }
}
