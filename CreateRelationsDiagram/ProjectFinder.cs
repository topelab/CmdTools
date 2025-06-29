using CmdTools.Contracts;
using System.Text;

namespace CreateRelationsDiagram
{
    internal class ProjectFinder : IElementFinder<Options>
    {
        protected readonly IProjectReferences projectReferences;
        protected readonly IFileExecutor fileExecutor;

        public ProjectFinder(IProjectReferences projectReferences, IFileExecutor fileExecutor)
        {
            this.projectReferences = projectReferences ?? throw new ArgumentNullException(nameof(projectReferences));
            this.fileExecutor = fileExecutor ?? throw new ArgumentNullException(nameof(fileExecutor));
        }

        public virtual void Run(Options options)
        {
            var path = options.SolutionPath ?? Environment.ProcessPath;
            var outputFile = options.OutputFile ?? Constants.RelationsFileName;
            var excludeProjects = options.Exclude ?? [];
            var projectFilter = options.ProjectFilter;

            Dictionary<string, HashSet<string>> references = [];
            fileExecutor.Initialize(path, Constants.FilePattern);
            fileExecutor.RunOnFiles(file => references[Path.GetFileNameWithoutExtension(file)] = projectReferences.Get(file).ToHashSet());

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
                    .ForEach(p => references[p].ToList().ForEach(r => welcomeProjects.Add(r)));
            }
            while (count != welcomeProjects.Count);

            var projectsToProcess = welcomeProjects
                .Where(p => references.ContainsKey(p))
                .ToList();

            int depth = GetDepth(projectsToProcess, references);
            int keysCount = projectsToProcess.Count;
            string direction = keysCount > depth ? "TD" : "LR";

            StringBuilder content = Initialize(direction);

            projectsToProcess.ForEach(project =>
            {
                foreach (var reference in references[project])
                {
                    content.AppendLine($"\t{project} --> {reference}");
                }
            });

            Finalize(content, outputFile);
        }

        protected StringBuilder Initialize(string direction)
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine("```mermaid");
            content.AppendLine("---");
            content.AppendLine("config:");
            content.AppendLine("  theme: default");
            content.AppendLine("---");
            content.AppendLine($"flowchart {direction}");
            return content;
        }

        protected void Finalize(StringBuilder content, string outputFile)
        {
            content.AppendLine("```");
            File.WriteAllText(outputFile, content.ToString());
            Console.WriteLine($"References diagram created at: {outputFile}");
        }

        protected int GetDepth(List<string> projectsToProcess, Dictionary<string, HashSet<string>> references)
        {
            return projectsToProcess
                .Select(p => GetDepth(p, references))
                .DefaultIfEmpty(0)
                .Max();
        }

        private int GetDepth(string project, Dictionary<string, HashSet<string>> references)
        {
            return !references.TryGetValue(project, out var value) 
                ? 0 
                : value.Select(r => GetDepth(r, references) + 1)
                    .DefaultIfEmpty(0)
                    .Max();
        }
    }
}
