namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;

    internal class ProjectFinder : ElementFinderBase, IElementFinder<Options>
    {
        protected readonly IProjectReferences projectReferences;
        protected readonly IFileExecutor fileExecutor;
        protected readonly IRelationGetterFactory relationGetterFactory;

        public ProjectFinder(IProjectReferences projectReferences, IFileExecutor fileExecutor, IRelationGetterFactory relationGetterFactory)
        {
            this.projectReferences = projectReferences ?? throw new ArgumentNullException(nameof(projectReferences));
            this.fileExecutor = fileExecutor ?? throw new ArgumentNullException(nameof(fileExecutor));
            this.relationGetterFactory = relationGetterFactory ?? throw new ArgumentNullException(nameof(relationGetterFactory));
        }

        public virtual void Run(Options options)
        {
            var path = options.SolutionPath ?? Environment.ProcessPath;
            var outputFile = options.OutputFile ?? Constants.RelationsFileName;
            var excludeProjects = options.Exclude ?? [];
            var projectFilter = options.ProjectFilter;
            projectReferences.Initialize(options.WithPackages);

            Dictionary<string, HashSet<string>> references = [];
            fileExecutor.Initialize(path, Constants.FilePattern);
            HashSet<string> projectFiles = [];
            fileExecutor.RunOnFiles(file =>
            {
                projectFiles.Add(file);
                projectReferences.GetProjects(file).ToList().ForEach(p => projectFiles.Add(p));
            });

            foreach (var file in projectFiles)
            {
                references[Path.GetFileNameWithoutExtension(file)] = [.. projectReferences.Get(file)];
            }

            var relationsGetter = relationGetterFactory.Create(options.FinderType);

            var content = relationsGetter.Get(
                references,
                excludeProjects,
                projectFilter);

            content = GetComposition(content, options.Theme, options.Layout, options.Direction);
            Finalize(content, outputFile);
        }
    }
}
