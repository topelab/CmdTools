using System.Text;

namespace CreateRelationsDiagram
{
    internal class ProjectFinder : IProjectFinder
    {
        private readonly IProjectReferences projectReferences;
        private readonly IFileExecutor fileExecutor;

        public ProjectFinder(IProjectReferences projectReferences, IFileExecutor fileExecutor)
        {
            this.projectReferences = projectReferences ?? throw new ArgumentNullException(nameof(projectReferences));
            this.fileExecutor = fileExecutor ?? throw new ArgumentNullException(nameof(fileExecutor));
        }

        public void Run(Options options)
        {
            var path = options.SolutionPath ?? Environment.ProcessPath;
            var outputFile = options.OutputFile ?? Constants.RelationsFileName;
            var excludeProjects = options.Exclude ?? [];
            var projectFilter = options.ProjectFilter;

            StringBuilder content = new StringBuilder("```mermaid\n---\nconfig:\n  theme: default\n---\nflowchart LR\n");
            fileExecutor.Initialize(path, Constants.FilePattern);
            Dictionary<string, List<string>> references = [];
            fileExecutor.RunOnFiles(file => references[Path.GetFileNameWithoutExtension(file)] = projectReferences.Get(file).ToList());

            HashSet<string> welcomeProjects = [];
            references.Keys
                .Where(p => !excludeProjects.Any(e => p.Contains(e, StringComparison.OrdinalIgnoreCase)))
                .Where(p => string.IsNullOrEmpty(projectFilter) || p.Contains(projectFilter, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(p => welcomeProjects.Add(p));

            int count = welcomeProjects.Count;

            do
            {
                count = welcomeProjects.Count;
                welcomeProjects
                    .Where(p => references.ContainsKey(p))
                    .ToList()
                    .ForEach(p => references[p].ForEach(r => welcomeProjects.Add(r)));
            }
            while (count != welcomeProjects.Count);

            welcomeProjects.OrderBy(p => p)
                .Distinct()
                .Where(p => references.ContainsKey(p))
                .ToList()
                .ForEach(project =>
                {
                    foreach (var reference in references[project])
                    {
                        content.AppendLine($"\t{project} --> {reference}");
                    }
                });

            content.AppendLine("```");

            if (content.Length > 0)
            {
                File.WriteAllText(outputFile, content.ToString());
                Console.WriteLine($"Project references diagram created at: {outputFile}");
            }
            else
            {
                Console.WriteLine("No project references found.");
            }
        }

        private bool CanRun(string project, string projectFilter, IEnumerable<string> excludeProjects)
        {
            if (string.IsNullOrEmpty(projectFilter) || project.Contains(projectFilter, StringComparison.OrdinalIgnoreCase))
            {
                return !excludeProjects.Any(e => project.Contains(e, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }
    }
}
