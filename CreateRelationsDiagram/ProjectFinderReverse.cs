namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using CmdTools.Shared;
    using System.Xml.Linq;

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
            var fixedProject = options.PinnedProject;

            projectReferences.Initialize(options.WithPackages);
            fileExecutor.Initialize(path, Constants.FilePattern);

            var projectFiles = GetProjectFiles();
            var filteredRefeferences = GetFilteredReferences(fixedProject, projectFiles, out var s);

            var relationsGetter = relationGetterFactory.Create(options.FinderType);

            var content = relationsGetter.Get(
                filteredRefeferences,
                excludeProjects,
                projectFilter);

            content = GetComposition(content, options.Theme, options.Layout, options.Direction);
            Finalize(content, outputFile);
        }
    }
}
