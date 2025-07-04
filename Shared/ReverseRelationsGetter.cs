namespace CmdTools.Shared
{
    using System.Text;

    internal class ReverseRelationsGetter : RelationsGetter
    {
        public override string Get(Dictionary<string, HashSet<string>> references, IEnumerable<string> excludedElements, string elementFilter)
        {
            HashSet<string> welcomeElements = [];

            references.Keys
                .Where(p => !excludedElements.Any(e => p.Contains(e, StringComparison.OrdinalIgnoreCase)))
                .Where(p => string.IsNullOrEmpty(elementFilter) || p.Contains(elementFilter, StringComparison.OrdinalIgnoreCase) || references[p].Any(r => r.Contains(elementFilter, StringComparison.OrdinalIgnoreCase)))
                .ToList()
                .ForEach(p => welcomeElements.Add(p));

            int count = welcomeElements.Count;

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
                .OrderBy(p => p)
                .Distinct()
                .Where(p => references.ContainsKey(p))
                .ToList();

            int depth = GetDepth(elementsToProcess, references);
            int keysCount = elementsToProcess.Count;
            string direction = keysCount > depth ? "LR" : "TD";
            StringBuilder content = Initialize(direction);


            List<(string element, string reference)> contentResult = [];
            elementsToProcess.ForEach(reference =>
            {
                foreach (var project in references[reference].Where(p => !excludedElements.Any(e => p.Contains(e, StringComparison.OrdinalIgnoreCase))))
                {
                    contentResult.Add((project, reference));
                }
            });

            contentResult
                .Where(p => string.IsNullOrEmpty(elementFilter) || p.element.Contains(elementFilter, StringComparison.OrdinalIgnoreCase) || p.reference.Contains(elementFilter, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.element)
                .ThenBy(p => p.reference)
                .ToList()
                .ForEach(p => content.AppendLine($"\t{p.element} --> {p.reference}"));

            content.AppendLine("```");

            return content.ToString();
        }
    }
}
