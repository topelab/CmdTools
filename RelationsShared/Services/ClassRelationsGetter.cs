namespace RelationsShared.Services
{
    using CmdTools.Contracts;
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using System.Collections.Generic;
    using System.Linq;

    internal class ClassRelationsGetter : IElementRelationsGetter
    {
        private readonly Dictionary<FinderType, Func<string, List<ElementRelation>, List<Relation>>> relationGetters = [];

        public ClassRelationsGetter()
        {
            relationGetters[FinderType.Classes] = GetRelations;
            relationGetters[FinderType.ReverseClasses] = GetInverseRelations;
        }

        public IEnumerable<Relation> GetFromContext(RelationsContext relationsContext)
        {
            var context = relationsContext as ClassRelationsContext;
            var options = context.Options as ClassOptions;

            List<Relation> relations = [.. context.ElementsRelations.Keys.SelectMany(er => relationGetters[options.FinderType](er, context.ElementsRelations[er]))];

            var relationsWithLevels = GetRelationsWithLevels(options.SelectedElement, relations)
                    .OrderBy(r => r.Level)
                    .ThenBy(r => r.Element)
                    .ThenBy(r => r.Reference)
                    .Distinct();

            return relationsWithLevels;
        }

        private List<Relation> GetRelations(string element, List<ElementRelation> elementRelations)
        {
            return [.. elementRelations.Select(er => new Relation { Element = element, Reference = er.Name, Level = 0 })];
        }

        private List<Relation> GetInverseRelations(string element, List<ElementRelation> elementRelations)
        {
            return [.. elementRelations.Select(er => new Relation { Element = er.Name, Reference = element, Level = 0 })];
        }

        private List<Relation> GetRelationsWithLevels(string element, IEnumerable<Relation> relations, HashSet<string> visitedElements = null, int level = 0)
        {
            List<Relation> result = [];
            visitedElements ??= [];
            bool isNew = visitedElements.Add(element);

            if (isNew)
            {
                result = [.. relations
                    .Where(r => r.Element == element)
                    .Select(r => new Relation { Element = element, Reference = r.Reference, Level = level + 1 })
                ];

                List<Relation> newRelations = [];


                foreach (Relation relation in result)
                {
                    newRelations.AddRange(GetRelationsWithLevels(relation.Reference, relations, visitedElements, level + 1));
                }
                result.AddRange(newRelations);
            }

            return result;
        }

    }
}