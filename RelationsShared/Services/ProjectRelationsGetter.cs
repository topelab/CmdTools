namespace RelationsShared.Services
{
    using CmdTools.Contracts;
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ProjectRelationsGetter : IElementRelationsGetter
    {
        private readonly IProjectReferences projectReferences;
        private readonly IRelationGetterFactory relationGetterFactory;
        private readonly Dictionary<FinderType, Func<bool, string, List<ElementRelation>, List<Relation>>> relationGetters = [];

        public ProjectRelationsGetter(IProjectReferences projectReferences,
                                              IRelationGetterFactory relationGetterFactory)
        {
            this.projectReferences = projectReferences;
            this.relationGetterFactory = relationGetterFactory;
            relationGetters[FinderType.Projects] = GetRelations;
            relationGetters[FinderType.ReverseProjects] = GetInverseRelations;
        }

        public IEnumerable<Relation> GetFromContext(RelationsContext relationsContext)
        {
            var context = relationsContext as ProjectRelationsContext;
            var options = context.Options as ProjectOptions;
            var relations = context.ElementsRelations.Keys
                .SelectMany(er => relationGetters[options.FinderType](options.WithPackages, Path.GetFileNameWithoutExtension(er), context.ElementsRelations[er]));

            var relationsWithLevels = GetRelationsWithLevels(options.SelectedElement, relations)
                    .DistinctBy(r => $"{r.Element}-{r.Reference}-{r.Level}")
                    .OrderBy(r => r.Level)
                    .ThenBy(r => r.Element)
                    .ThenBy(r => r.Reference);

            return relationsWithLevels;
        }

        private List<Relation> GetRelations(bool withPackages, string element, List<ElementRelation> elementRelations)
        {
            return [.. elementRelations
                .Where(er => withPackages || er.RelationType != ProjectRelationType.PackageReference)
                .Select(er => new Relation { Element = element, Reference = er.ToString(), Level = 0 })];
        }

        private List<Relation> GetInverseRelations(bool withPackages, string element, List<ElementRelation> elementRelations)
        {
            return [.. elementRelations
                .Where(er => withPackages || er.RelationType != ProjectRelationType.PackageReference)
                .Select(er => new Relation { Element = er.ToString(), Reference = element, Level = 0 })];
        }

        private List<Relation> GetRelationsWithLevels(string element, IEnumerable<Relation> relations, HashSet<string> visitedElements = null, int level = 0)
        {
            List<Relation> result = [];
            visitedElements ??= [];
            bool isNew = visitedElements.Add(element);

            result = [.. relations
                .Where(r => r.Element == element)
                .Select(r => new Relation { Element = element, Reference = r.Reference, Level = level + 1 })
            ];

            if (isNew)
            {
                List<Relation> newRelations = [];

                foreach (Relation relation in result)
                {
                    newRelations.AddRange(GetRelationsWithLevels(relation.Reference, relations, visitedElements, level + 1));
                }
                result.AddRange(newRelations);
            }

            return result;
        }


        private IEnumerable<Relation> GetFromContextOld(RelationsContext relationsContext)
        {
            var context = relationsContext as ProjectRelationsContext;
            var options = context.Options as ProjectOptions;
            projectReferences.Initialize(context);
            var filteredReferences = GetFilteredReferences(options.PinnedElement, context.Elements);
            var relationsGetter = relationGetterFactory.Create(options.FinderType);
            return relationsGetter.Get<Relation>(filteredReferences, options.ProjectFilter);
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
