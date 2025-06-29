using CmdTools.Contracts;

namespace CreateRelationsDiagram
{
    internal class ProjectFinder : IElementFinder<Options>
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

            Dictionary<string, HashSet<string>> references = [];
            fileExecutor.Initialize(path, Constants.FilePattern);
            fileExecutor.RunOnFiles(file => references[Path.GetFileNameWithoutExtension(file)] = projectReferences.Get(file).ToHashSet());
            var relationsGetter = relationGetterFactory.Create(options.FinderType);

            var content = relationsGetter.Get(
                references,
                excludeProjects,
                projectFilter);

            Finalize(content, outputFile);
        }

        protected void Finalize(string content, string outputFile)
        {
            File.WriteAllText(outputFile, content);
            Console.WriteLine($"References diagram created at: {outputFile}");
        }
    }
}
