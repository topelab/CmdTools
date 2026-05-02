namespace RelationsShared.Services
{
    using CmdTools.Contracts;
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class ProjectRelationsGetter(IProjectReferences projectReferences,
                                          IFileExecutor fileExecutor,
                                          IRelationGetterFactory relationGetterFactory) : IElementRelationsGetter
    {
        private readonly IProjectReferences projectReferences = projectReferences;
        private readonly IFileExecutor fileExecutor = fileExecutor;
        private readonly IRelationGetterFactory relationGetterFactory = relationGetterFactory;

        public IEnumerable<Relation> Get<T>(T args) where T : class
        {
            var options = GetOptions(args);
            var path = options.RootPath ?? Environment.ProcessPath;
            var excludeProjects = string.IsNullOrEmpty(options.Exclude) ? null : new Regex(options.Exclude, RegexOptions.IgnoreCase);
            var projectFilter = options.ProjectFilter;
            options.OutputFile = options.OutputFile?.Replace(".csproj", string.Empty, StringComparison.CurrentCultureIgnoreCase);
            options.PinnedElement = options.PinnedElement?.Replace(".csproj", string.Empty, StringComparison.CurrentCultureIgnoreCase);

            projectReferences.Initialize(options.WithPackages, excludeProjects);

            var projectFiles = GetProjectFiles(path, excludeProjects);
            options.ProjectPaths.Clear();
            options.ProjectPaths.AddRange(projectFiles);
            var filteredReferences = GetFilteredReferences(options.PinnedElement, projectFiles);

            var relationsGetter = relationGetterFactory.Create(options.FinderType);
            return relationsGetter.Get<Relation>(filteredReferences, projectFilter);
        }

        private ProjectOptions GetOptions<T>(T args) where T : class
        {
            if (args is not ProjectOptions options)
            {
                throw new ArgumentException("Invalid options type", nameof(args));
            }

            return options;
        }

        private HashSet<string> GetProjectFiles(string path, Regex excludeProjects)
        {
            fileExecutor.Initialize(path, Constants.FilePattern, excludeProjects);

            HashSet<string> projectFiles = [];
            fileExecutor.RunOnFiles(file =>
            {
                projectFiles.Add(file);
                projectReferences.GetProjects(file).ToList().ForEach(p => projectFiles.Add(p));
            });
            return projectFiles;
        }

        private ReferencesBag GetFilteredReferences(string pinnedElement, HashSet<string> projectFiles)
        {
            ReferencesBag filteredReferences = [];

            if (!string.IsNullOrEmpty(pinnedElement))
            {
                var inverseReferences = projectReferences.GetInverseReferences(projectFiles);
                var selectedElement = FindPinnedElement(inverseReferences, pinnedElement);
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
