namespace ProjectRelations2022.Services
{
    using CreateRelationsDiagram;
    using ProjectRelations2022.Views;

    internal interface IProjectRelationsOpener
    {
        Task<RelationsUserControlContext> OpenUsedByProjectAsync(string projectPath, string projectName);
        Task<RelationsUserControlContext> OpenUsingProjectAsync(string solutionPath, string projectName);
    }
}