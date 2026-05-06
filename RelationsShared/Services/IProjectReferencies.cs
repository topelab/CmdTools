namespace RelationsShared.Services
{
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using System.Text.RegularExpressions;

    internal interface IProjectReferences
    {
        ReferencesBag GetInverseReferences(HashSet<string> projectFiles);
        IEnumerable<ElementRelation> GetProjectRelations(string projectPath, HashSet<string> currentProjects = null, string basePath = null);
        IEnumerable<string> GetProjects(string file, HashSet<string> currentProjects = null, string basePath = null);
        ReferencesBag GetReferences(HashSet<string> projectFiles);
        void Initialize(bool withPackages, Regex excludeProjects, string path);
        void Initialize(ProjectRelationsContext context);
    }
}
