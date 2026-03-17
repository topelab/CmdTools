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
            try
            {
                var projectPath = await context.GetSelectedPathAsync(cancellationToken);

                // Validar que la ruta seleccionada no sea nula
                if (projectPath?.LocalPath == null)
                {
                    throw new InvalidOperationException("No se seleccionó ningún proyecto. Por favor, selecciona un proyecto en el Solution Explorer.");
                }

                var itemPath = Path.GetDirectoryName(projectPath.LocalPath);

                var allProjects = await workspace.QueryProjectsAsync(
                    project => project.With(p => new { p.Name, p.Guid, p.Path }),
                    cancellationToken);

                var results = allProjects.ToList().Where(
                    project => itemPath.Contains(Path.GetDirectoryName(project.Path), StringComparison.InvariantCultureIgnoreCase))
                    .Select(p => new { p.Name, p.Guid, p.Path })
                    .ToList();

                var projectSnapshot = results.FirstOrDefault();

                // Validar que se encontró el proyecto
                if (projectSnapshot == null)
                {
                    throw new InvalidOperationException($"No se encontró el proyecto en la ruta: {projectPath.LocalPath}");
                }

                var solutions = await workspace.QuerySolutionAsync(
                    solution => solution.With(s => new { s.BaseName, s.Path, s.Projects }),
                    cancellationToken);

                var solution = solutions.FirstOrDefault();

                // Validar que se encontró la solución
                if (solution == null)
                {
                    throw new InvalidOperationException("No se encontró la solución actual.");
                }

                return new SolutionExplorerItem(projectSnapshot.Name, Path.GetDirectoryName(projectSnapshot.Path), Path.GetDirectoryName(solution.Path), allProjects.ToDictionary(p => p.Name, p => p.Path));
            }
            catch (OperationCanceledException)
            {
                throw new InvalidOperationException("La operación fue cancelada por el usuario.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener los detalles del proyecto: {ex.Message}", ex);
            }
        }
    }
}
