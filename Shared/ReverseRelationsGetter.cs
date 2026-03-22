namespace CmdTools.Shared
{
    using CmdTools.Contracts;
    using System.Text;

    internal class ReverseRelationsGetter : RelationsGetter
    {
        public override IEnumerable<TRelation> Get<TRelation>(IReferencesBag references, string elementFilter)
        {
            HashSet<string> welcomeElements = [];

            references.Keys
                .Where(p => string.IsNullOrEmpty(elementFilter) || p.Contains(elementFilter, StringComparison.CurrentCultureIgnoreCase))
                .ToList()
                .ForEach(p => welcomeElements.Add(p));

            int count;

            do
            {
                count = welcomeElements.Count;
                welcomeElements
                    .Where(p => references.ContainsKey(p))
                    .ToList()
                    .ForEach(p => references[p].ToList().ForEach(r => welcomeElements.Add(r)));
            }
            while (count != welcomeElements.Count);

            var elementsToProcess = welcomeElements
                .Where(p => references.ContainsKey(p))
                .ToList();

            var content = new StringBuilder();

            List<TRelation> contentResult = [];
            elementsToProcess.ForEach(element => contentResult.AddRange(references[element].Select(reference => new TRelation { Element = reference, Reference = element })));

            return contentResult
                .Where(p => string.IsNullOrEmpty(elementFilter) || p.Element.Contains(elementFilter, StringComparison.CurrentCultureIgnoreCase) || p.Reference.Contains(elementFilter, StringComparison.CurrentCultureIgnoreCase))
                .OrderBy(p => p.Element)
                .ThenBy(p => p.Reference);
        }
    }
}
