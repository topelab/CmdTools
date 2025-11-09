namespace ProjectRelations2022.Services
{
    using Microsoft.VisualStudio.Extensibility;
    using Microsoft.VisualStudio.ProjectSystem.Query;
    using ProjectRelations2022.DTO;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class CommandExtensions
    {
        public static async Task<SolutionExplorerItem> GetSelectedProjectDetailsAsync(this WorkspacesExtensibility workspace, IClientContext context, CancellationToken cancellationToken)
        {
            var projectPath = await context.GetSelectedPathAsync(cancellationToken);

            var allProjects = await workspace.QueryProjectsAsync(
                project => project.With(p => new { p.Name, p.Guid, p.Path }),
                cancellationToken);

            var results = await workspace.QueryProjectsAsync(
                project => project.Where(p => p.Path == projectPath.LocalPath).With(p => new { p.Name, p.Guid, p.Path }),
                cancellationToken);

            var projectSnapshot = results.FirstOrDefault();

            var solutions = await workspace.QuerySolutionAsync(
                solution => solution.With(s => new { s.BaseName, s.Path, s.Projects }),
                cancellationToken);

            var solution = solutions.First();

            return new SolutionExplorerItem(projectSnapshot.Name, Path.GetDirectoryName(projectSnapshot.Path), Path.GetDirectoryName(solution.Path), allProjects.ToDictionary(p => p.Name, p => p.Path));
        }
    }
}
