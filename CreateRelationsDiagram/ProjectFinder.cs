namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using CmdTools.Shared;
    using System.Text.RegularExpressions;

    internal class ProjectFinder : ElementFinderBase, IElementFinder
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

        public virtual void Run<T>(T args) where T : class
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

        protected HashSet<string> GetProjectFiles()
        {
            HashSet<string> projectFiles = [];
            fileExecutor.RunOnFiles(file =>
            {
                projectFiles.Add(file);
                projectReferences.GetProjects(file).ToList().ForEach(p => projectFiles.Add(p));
            });
            return projectFiles;
        }

        protected virtual ReferencesBag GetFilteredReferences(string pinnedElement, HashSet<string> projectFiles, out string selectedElement)
        {
            ReferencesBag filteredRefeferences = [];
            selectedElement = null;

            if (!string.IsNullOrEmpty(pinnedElement))
            {
                var inverseReferences = GetInverseReferences(projectFiles);
                selectedElement = FindPinnedElement(inverseReferences, pinnedElement);
                if (selectedElement != null)
                {
                    SetInverseReferences(inverseReferences, selectedElement, selectedElement, filteredRefeferences);
                }
            }
            else
            {
                filteredRefeferences = GetReferences(projectFiles);
            }

            return filteredRefeferences;
        }

        protected virtual void SetReferences(ReferencesBag references, string parentElement, string pinnedElement, ReferencesBag currentResults, HashSet<(string, string)> processed = null)
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
                    .Where(e => e == pinnedElement)
                    .ToList().ForEach(e => results.AddReference(parentElement, e));

                elements
                    .ToList().ForEach(e => SetReferences(references, e, pinnedElement, results, processed));

                foreach (var key in results.Keys.ToList())
                {
                    SetReferences(references, parentElement, key, results, processed);
                }
            }

            results.Keys
                .ToList()
                .ForEach(key => currentResults.AddReferences(key, results[key]));

        }

        protected virtual void SetInverseReferences(ReferencesBag references, string parentElement, string pinnedElement, ReferencesBag currentResults, HashSet<(string, string)> processed = null)
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
                    .ToList().ForEach(e => SetInverseReferences(references, e, pinnedElement, results, processed));

            }

            results.Keys
                .ToList()
                .ForEach(key => currentResults.AddReferences(key, results[key]));

        }

        protected virtual ReferencesBag GetReferences(HashSet<string> projectFiles)
        {
            ReferencesBag references = [];
            foreach (var file in projectFiles)
            {
                references[Path.GetFileNameWithoutExtension(file)] = [.. projectReferences.Get(file)];
            }

            return references;
        }

        protected virtual ReferencesBag GetInverseReferences(HashSet<string> projectFiles)
        {
            ReferencesBag references = [];
            foreach (var file in projectFiles)
            {
                this.projectReferences.Get(file).ToList().ForEach(r => references.AddReference(r, Path.GetFileNameWithoutExtension(file)));
            }

            return references;
        }
    }
}
