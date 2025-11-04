namespace ProjectRelations2022.Services
{
    internal interface IProjectRelationsOpener
    {
        Task OpenUsedByProjectAsync(string projectPath, string projectName);
        Task OpenUsingProjectAsync(string solutionPah, string projectName);
    }
}