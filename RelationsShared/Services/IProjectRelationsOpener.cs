namespace RelationsShared.Services
{
    using RelationsShared.DTO;

    public interface IProjectRelationsOpener
    {
        Task<RelationsUserControlContext> OpenAsync(RelationType relationType, SolutionExplorerItem solutionExplorerItem);
    }
}