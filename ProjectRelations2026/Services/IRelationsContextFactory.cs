namespace ProjectRelations2026.Services
{
    using ProjectRelations2026.Views;
    using RelationsShared.DTO;

    public interface IRelationsContextFactory
    {
        Task<RelationsContext> CreateAsync(RelationType relationType, SolutionExplorerItem solutionExplorerItem);
    }
}