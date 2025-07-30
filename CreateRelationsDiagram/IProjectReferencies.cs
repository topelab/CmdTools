namespace CreateRelationsDiagram
{
    internal interface IProjectReferences
    {
        IEnumerable<string> Get(string projectPath);
        IEnumerable<string> GetProjects(string file, HashSet<string> currentProjects = null, string basePath = null);
        void Initialize(bool withPackages);
    }
}
