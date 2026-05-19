namespace RelationsShared.DTO
{
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class ProjectRelationsContext
    {
        public ProjectOptions Options { get; set; }
        public ElementsRelations ProjectRelations { get; set; } = [];
        public Dictionary<string, string> PackageVersions { get; set; } = [];
        public HashSet<string> PackageSets { get; internal set; } = [];
        public HashSet<string> Projects { get; set; } = [];
        public Regex ExcludeProjects { get; internal set; }
    }
}
