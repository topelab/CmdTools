namespace CreateRelationsDiagram
{
    using CmdTools.Shared;

    internal interface IProjectReferences
    {
        IEnumerable<string> Get(string projectPath);
        ReferencesBag GetInverseReferences(HashSet<string> projectFiles);
        IEnumerable<string> GetProjects(string file, HashSet<string> currentProjects = null, string basePath = null);
        ReferencesBag GetReferences(HashSet<string> projectFiles);
        void Initialize(bool withPackages);
    }
}
