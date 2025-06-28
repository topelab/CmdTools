using System.Text;

namespace CreateRelationsDiagram
{
    internal class ProjectFinderReverse : IProjectFinder
    {
        private readonly IProjectReferences projectReferences;
        private readonly IFileExecutor fileExecutor;

        public ProjectFinderReverse(IProjectReferences projectReferences, IFileExecutor fileExecutor)
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

            if (string.IsNullOrEmpty(projectFilter))
            {
                Console.WriteLine("ProjectFilter (-p --project) is mandatory when reverse option is set");
                return;
            }

            StringBuilder content = new StringBuilder("```mermaid\n---\nconfig:\n  theme: default\n---\nflowchart LR\n");
            fileExecutor.Initialize(path, Constants.FilePattern);
            Dictionary<string, HashSet<string>> references = [];

            fileExecutor.RunOnFiles(file =>
            {
                projectReferences.Get(file)
                    .ToList()
                    .ForEach(reference => 
                    {
                        if (!references.TryGetValue(reference, out var projects))
                        {
                            projects = [];
                            references[reference] = projects;
                        }

                        projects.Add(Path.GetFileNameWithoutExtension(file));
                    });
            });

            HashSet<string> welcomeReferences = [];

            references.Keys
                .Where(p => !excludeProjects.Any(e => p.Contains(e, StringComparison.OrdinalIgnoreCase)))
                .Where(p => p.Contains(projectFilter, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(p => welcomeReferences.Add(p));

            int count = welcomeReferences.Count;

            do
            {
                count = welcomeReferences.Count;
                welcomeReferences
                    .Where(p => references.ContainsKey(p))
                    .ToList()
                    .ForEach(p => references[p].ToList().ForEach(r => welcomeReferences.Add(r)));
            }
            while (count != welcomeReferences.Count);

            welcomeReferences.OrderBy(p => p)
                .Distinct()
                .Where(p => references.ContainsKey(p))
                .ToList()
                .ForEach(reference =>
                {
                    foreach (var project in references[reference])
                    {
                        content.AppendLine($"\t{project} --> {reference}");
                    }
                });

            content.AppendLine("```");

            if (content.Length > 0)
            {
                File.WriteAllText(outputFile, content.ToString());
                Console.WriteLine($"Reverse project references diagram created at: {outputFile}");
            }
            else
            {
                Console.WriteLine("No project references found.");
            }
        }
    }
}
