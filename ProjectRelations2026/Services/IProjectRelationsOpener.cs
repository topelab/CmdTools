namespace ProjectRelations2026.Services
{
    using CreateRelationsDiagram;
    using ProjectRelations2026.DTO;
    using ProjectRelations2026.Views;

    internal interface IProjectRelationsOpener
    {
        Task<RelationsUserControlContext> OpenAsync(RelationType relationType, SolutionExplorerItem solutionExplorerItem);
        //Task<RelationsUserControlContext> OpenUsedByProjectAsync(string projectPath, string projectName);
        //Task<RelationsUserControlContext> OpenUsingProjectAsync(string solutionPath, string projectName);
    }
}