namespace CmdTools.Shared
{
    using CmdTools.Contracts;
    using System.Text;

    internal class RelationsGetter : IRelationsGetter
    {
        public virtual string Get(IReferencesBag references, IEnumerable<string> excludedElements, string elementFilter)
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

            List<(string element, string reference)> contentResult = [];
            elementsToProcess.ForEach(element => contentResult.AddRange(references[element].Select(reference => (element, reference))));

            contentResult
                .Where(p => string.IsNullOrEmpty(elementFilter) || p.element.Contains(elementFilter, StringComparison.CurrentCultureIgnoreCase) || p.reference.Contains(elementFilter, StringComparison.CurrentCultureIgnoreCase))
                .Where(p => !excludedElements.Any(e => p.element.Contains(e, StringComparison.CurrentCultureIgnoreCase) || p.reference.Contains(e, StringComparison.CurrentCultureIgnoreCase)))
                .OrderBy(p => p.element)
                .ThenBy(p => p.reference)
                .ToList()
                .ForEach(p => content.AppendLine($"\t{p.element} --> {p.reference}"));

            return content.ToString();
        }
    }
}
