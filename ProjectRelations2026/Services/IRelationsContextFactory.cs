namespace ProjectRelations2026.Services
{
    using RelationsShared.DTO;

    public interface IRelationsContextFactory
    {
        Task<RelationsContext> CreateAsync(RelationType relationType, SolutionExplorerItem solutionExplorerItem);
    }
}