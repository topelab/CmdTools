namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using CmdTools.Shared;
    using System.Text.RegularExpressions;

    internal class ProjectFinder : ElementFinderBase, IElementFinder
    {
        private readonly IProjectReferences projectReferences;
        private readonly IFileExecutor fileExecutor;
        private readonly IRelationGetterFactory relationGetterFactory;

        public ProjectFinder(IProjectReferences projectReferences, IFileExecutor fileExecutor, IRelationGetterFactory relationGetterFactory)
        {
            this.projectReferences = projectReferences ?? throw new ArgumentNullException(nameof(projectReferences));
            this.fileExecutor = fileExecutor ?? throw new ArgumentNullException(nameof(fileExecutor));
            this.relationGetterFactory = relationGetterFactory ?? throw new ArgumentNullException(nameof(relationGetterFactory));
        }

        public void Run<T>(T args) where T : class
        {
            if (args is not ProjectOptions options)
            {
                throw new ArgumentException("Invalid options type", nameof(args));
            }

            var path = options.RootPath ?? Environment.ProcessPath;
            var outputFile = options.OutputFile;
            var excludeProjects =string.IsNullOrEmpty(options.Exclude) ? null : new Regex(options.Exclude);
            var projectFilter = options.ProjectFilter;
            var pinnedProject = options.PinnedProject;

            projectReferences.Initialize(options.WithPackages);
            fileExecutor.Initialize(path, Constants.FilePattern, excludeProjects);

            var projectFiles = GetProjectFiles();
            var filteredRefeferences = GetFilteredReferences(pinnedProject, projectFiles, out var selectedElement);

            var relationsGetter = relationGetterFactory.Create(options.FinderType);

            var content = relationsGetter.Get(
                filteredRefeferences,
                projectFilter);

            content = GetComposition(content, options.Theme, options.Layout, options.Direction);
            if (!string.IsNullOrEmpty(selectedElement))
            {
                content = content.Replace($"\t{selectedElement} ", $"\t{selectedElement}:::pinned");
                content = content.Replace($":::pkg:::pinned", $":::pinnedpkg");
            }
            Finalize(content, outputFile);
        }

        private HashSet<string> GetProjectFiles()
        {
            HashSet<string> projectFiles = [];
            fileExecutor.RunOnFiles(file =>
            {
                projectFiles.Add(file);
                projectReferences.GetProjects(file).ToList().ForEach(p => projectFiles.Add(p));
            });
            return projectFiles;
        }

        private ReferencesBag GetFilteredReferences(string pinnedElement, HashSet<string> projectFiles, out string selectedElement)
        {
            ReferencesBag filteredReferences = [];
            selectedElement = null;

            if (!string.IsNullOrEmpty(pinnedElement))
            {
                var inverseReferences = projectReferences.GetInverseReferences(projectFiles);
                selectedElement = FindPinnedElement(inverseReferences, pinnedElement);
                if (selectedElement != null)
                {
                    SetReferences(inverseReferences, selectedElement, selectedElement, filteredReferences);
                }
            }
            else
            {
                filteredReferences = projectReferences.GetReferences(projectFiles);
            }

            return filteredReferences;
        }

        private void SetReferences(ReferencesBag references, string parentElement, string pinnedElement, ReferencesBag currentResults, HashSet<(string, string)> processed = null)
        {
            processed ??= [];

            if (string.IsNullOrEmpty(parentElement) || string.IsNullOrEmpty(pinnedElement) || processed.Contains((parentElement, pinnedElement)))
            {
                return;
            }

            ReferencesBag results = [];
            if (references.TryGetValue(parentElement, out var elements))
            {
                processed.Add((parentElement, pinnedElement));

                elements
                    .ToList().ForEach(e => results.AddReference(parentElement, e));

                elements
                    .ToList().ForEach(e => SetReferences(references, e, pinnedElement, results, processed));

            }

            results.Keys
                .ToList()
                .ForEach(key => currentResults.AddReferences(key, results[key]));

        }

        private string FindPinnedElement(ReferencesBag references, string pinnedElement)
        {
            var pinned =
                references.Keys.FirstOrDefault(p => p.Equals(pinnedElement, StringComparison.CurrentCultureIgnoreCase))
                    ?? references.Keys.FirstOrDefault(p => p.Split('-')[0].Equals(pinnedElement, StringComparison.CurrentCultureIgnoreCase));

            if (pinned == null)
            {
                if (pinnedElement.Contains('-'))
                {
                    pinned = references.Keys.FirstOrDefault(p => p.StartsWith(pinnedElement, StringComparison.CurrentCultureIgnoreCase))
                        ?? references.SelectMany(r => r.Value).FirstOrDefault(r => r.StartsWith(pinnedElement, StringComparison.CurrentCultureIgnoreCase));
                }
                else
                {
                    var allReferences = references.SelectMany(r => r.Value).ToList();
                    pinned = allReferences.FirstOrDefault(r => r.Equals(pinnedElement, StringComparison.CurrentCultureIgnoreCase))
                        ?? allReferences.FirstOrDefault(r => r.Split('-')[0].Equals(pinnedElement, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            return pinned;
        }
    }
}
