namespace CreateRelationsDiagram
{
    internal interface IProjectReferences
    {
        IEnumerable<string> Get(string projectPath);
    }
}
