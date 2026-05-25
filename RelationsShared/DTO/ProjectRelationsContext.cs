namespace RelationsShared.DTO
{
    using System.Collections.Generic;

    public class ProjectRelationsContext : RelationsContext
    {
        public Dictionary<string, string> PackageVersions { get; set; } = [];
        public HashSet<string> PackageSets { get; internal set; } = [];
    }
}
