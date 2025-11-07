namespace ProjectRelations2022.Services
{
    using CreateRelationsDiagram;
    using ProjectRelations2022.Views;

    internal interface IProjectRelationsOpener
    {
        Task<RelationsWindowsContext> OpenUsedByProjectAsync(string projectPath, string projectName);
        Task<RelationsWindowsContext> OpenUsingProjectAsync(string solutionPath, string projectName);
    }
}