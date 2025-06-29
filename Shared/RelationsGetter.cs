using CmdTools.Contracts;
using System.Text;

namespace CmdTools.Shared
{
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

            int depth = 10; //GetDepth(elementsToProcess, references);
            int keysCount = elementsToProcess.Count;
            string direction = keysCount > depth ? "TD" : "LR";

            StringBuilder content = Initialize(direction);

            elementsToProcess.ForEach(element =>
            {
                foreach (var reference in references[element])
                {
                    content.AppendLine($"\t{element} --> {reference}");
                }
            });

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
            return elementsToProcess
                .Select(p => GetDepth(p, references))
                .DefaultIfEmpty(0)
                .Max();
        }
        private int GetDepth(string element, Dictionary<string, HashSet<string>> references)
        {
            return !references.TryGetValue(element, out var value)
                ? 0
                : value.Select(r => GetDepth(r, references) + 1)
                    .DefaultIfEmpty(0)
                    .Max();
        }

    }
}
