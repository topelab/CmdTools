using CmdTools.Contracts;

namespace CreateRelationsDiagram
{
    internal class ProjectFinderReverse : ProjectFinder
    {
        public ProjectFinderReverse(IProjectReferences projectReferences, IFileExecutor fileExecutor, IRelationGetterFactory relationGetterFactory) : base(projectReferences, fileExecutor, relationGetterFactory)
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

            var relationsGetter = relationGetterFactory.Create(options.FinderType);

            var content = relationsGetter.Get(
                references,
                excludeProjects,
                projectFilter);

            Finalize(content, outputFile);
        }
    }
}
