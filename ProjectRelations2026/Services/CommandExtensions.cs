namespace ProjectRelations2026.Services
{
    using Microsoft.VisualStudio.Extensibility;
    using Microsoft.VisualStudio.ProjectSystem.Query;
    using ProjectRelations2026.DTO;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class CommandExtensions
    {
        public static async Task<SolutionExplorerItem> GetSelectedProjectDetailsAsync(this WorkspacesExtensibility workspace, IClientContext context, CancellationToken cancellationToken)
        {
            var selectedItem = await context.GetSelectedPathAsync(cancellationToken);

            if (selectedItem?.LocalPath == null)
            {
                throw new InvalidOperationException("No se seleccionó ningún proyecto. Por favor, selecciona un proyecto en el Solution Explorer.");
            }

            var selectedItemPath = Path.GetDirectoryName(selectedItem.LocalPath);

            var allProjects = (await workspace.QueryProjectsAsync(
                project => project.With(p => new { p.Name, p.Guid, p.Path }),
                cancellationToken)).ToList();

            var results = allProjects.Where(
                project => selectedItemPath.Contains(Path.GetDirectoryName(project.Path), StringComparison.InvariantCultureIgnoreCase))
                .Select(p => new { p.Name, p.Guid, p.Path })
                .ToList();

            var projectSnapshot = results.FirstOrDefault() ?? throw new InvalidOperationException($"No se encontró el proyecto en la ruta: {selectedItem.LocalPath}");

            var solutions = await workspace.QuerySolutionAsync(
                solution => solution.With(s => new { s.BaseName, s.Path, s.Projects }),
                cancellationToken);

            var solution = solutions.FirstOrDefault() ?? throw new InvalidOperationException("No se encontró la solución actual.");

            return new SolutionExplorerItem(projectSnapshot.Name, Path.GetDirectoryName(projectSnapshot.Path), Path.GetDirectoryName(solution.Path), allProjects.ToDictionary(p => p.Name, p => p.Path));
        }
    }
}
