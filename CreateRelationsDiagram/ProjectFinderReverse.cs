using System.Text;

namespace CreateRelationsDiagram
{
    internal class ProjectFinderReverse : ProjectFinder
    {
        public ProjectFinderReverse(IProjectReferences projectReferences, IFileExecutor fileExecutor) : base(projectReferences, fileExecutor)
        {
        }

        public override void Run(Options options)
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
                .Where(p => p.Contains(projectFilter, StringComparison.OrdinalIgnoreCase) || references[p].Any(r => r.Contains(projectFilter, StringComparison.OrdinalIgnoreCase)))
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

            var projectsToProcess = welcomeReferences
                .OrderBy(p => p)
                .Distinct()
                .Where(p => references.ContainsKey(p))
                .ToList();

            int depth = GetDepth(projectsToProcess, references);
            int keysCount = projectsToProcess.Count;
            string direction = keysCount > depth ? "BT" : "RL";
            StringBuilder content = Initialize(direction);


            projectsToProcess.ForEach(reference =>
            {
                foreach (var project in references[reference].Where(p => !excludeProjects.Any(e => p.Contains(e, StringComparison.OrdinalIgnoreCase))))
                {
                    content.AppendLine($"\t{project} --> {reference}");
                }
            });

            Finalize(content, outputFile);
        }
    }
}
