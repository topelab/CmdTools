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

            List<Relation> relations = [.. context.ElementsRelations.Keys.SelectMany(er => relationGetters[FinderType.Classes](er, context.ElementsRelations[er]))];
            return relations;
        }

        private List<Relation> GetRelations(string element, List<ElementRelation> elementRelations)
        {
            return [.. elementRelations.Select(er => new Relation { Element = element, Reference = er.Name, Level = 0 })];
        }

        private List<Relation> GetInverseRelations(string element, List<ElementRelation> elementRelations)
        {
            return [.. elementRelations.Select(er => new Relation { Element = element, Reference = er.Name, Level = 0 })];
        }
    }
}