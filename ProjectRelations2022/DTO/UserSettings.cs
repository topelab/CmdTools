namespace ProjectRelations2022.DTO
{
    using Newtonsoft.Json;

    public class UserSettings
    {
        public string CreateRelationsDiagramPath { get; set; }
        public bool ShowPackages { get; set; } = true;
        public string Theme { get; set; } = "Dark";
        public string BackgroundColor { get; set; } = "black";
        public int MaxEdges { get; set; } = 500;
        public long MaxTextSize { get; set; } = 50000;
        public string ExcludeProjects { get; set; } = "test";


        [JsonIgnore]
        public bool HasTheme => !string.IsNullOrEmpty(Theme);
        [JsonIgnore]
        public bool HasBackgroundColor => !string.IsNullOrEmpty(BackgroundColor);
    }
}
