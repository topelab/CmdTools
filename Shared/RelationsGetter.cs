using System.Linq;

namespace CmdTools.Shared
{
    using CmdTools.Contracts;
    using System.Text;

    internal class RelationsGetter : IRelationsGetter
    {
        public virtual string Get(Dictionary<string, HashSet<string>> references, IEnumerable<string> excludedElements, string elementFilter)
        {
            HashSet<string> welcomeElements = [];
            references.Keys
                .Where(p => !excludedElements.Any(e => p.Contains(e, StringComparison.OrdinalIgnoreCase)))
                .Where(p => string.IsNullOrEmpty(elementFilter) || p.Contains(elementFilter, StringComparison.OrdinalIgnoreCase))
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
                .Where(p => references.ContainsKey(p))
                .ToList();

            int depth = GetDepth(elementsToProcess, references);
            int keysCount = elementsToProcess.Count;
            string direction = keysCount > depth ? "LR" : "TD";

            StringBuilder content = Initialize(direction);

            List<(string element, string reference)> contentResult = [];
            elementsToProcess.ForEach(element => contentResult.AddRange(references[element].Select(reference => (element, reference))));

            contentResult
                .Where(p => string.IsNullOrEmpty(elementFilter) || p.element.Contains(elementFilter, StringComparison.OrdinalIgnoreCase) || p.reference.Contains(elementFilter, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.element)
                .ThenBy(p => p.reference)
                .ToList()
                .ForEach(p => content.AppendLine($"\t{p.element} --> {p.reference}"));

            content.AppendLine("```");

            return content.ToString();
        }

        protected StringBuilder Initialize(string direction)
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine("```mermaid");
            content.AppendLine("---");
            content.AppendLine("config:");
            content.AppendLine("  theme: default");
            content.AppendLine("---");
            content.AppendLine($"flowchart {direction}");
            return content;
        }

        protected int GetDepth(List<string> elementsToProcess, Dictionary<string, HashSet<string>> references)
        {
            HashSet<string> visited = [];
            return elementsToProcess
                .Select(p => GetDepth(p, references, visited))
                .DefaultIfEmpty(0)
                .Max();
        }
        private int GetDepth(string element, Dictionary<string, HashSet<string>> references, HashSet<string> visited)
        {
            if (visited.Contains(element))
            {
                return -1;
            }
            visited.Add(element);
            return !references.TryGetValue(element, out var value)
                ? 0
                : value.Select(r => GetDepth(r, references, visited) + 1)
                    .DefaultIfEmpty(0)
                    .Max();
        }

    }
}
