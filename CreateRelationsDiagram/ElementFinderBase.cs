namespace CreateRelationsDiagram
{
    using CmdTools.Shared;

    internal class ElementFinderBase
    {

        protected string GetComposition(string content, Theme theme, Layout layout, Direction direction)
        {
            return $"""
                ---
                config:
                  theme: {theme.GetDescription()}
                  layout: {layout.GetDescription()}
                ---
                flowchart {direction.GetDescription()}
                {content}

                classDef pkg fill:#658;
                classDef pinned stroke-width:10px;
                classDef pinnedpkg fill:#658, stroke-width:10px;
                """;
        }

        protected void Finalize(string content, string outputFile)
        {
            if (string.IsNullOrEmpty(outputFile))
            {
                Console.WriteLine(content);
            }
            else
            {
                File.WriteAllText(outputFile, content);
                Console.WriteLine($"Diagram created at: {outputFile}");
            }
        }

        protected string FindPinnedElement(ReferencesBag references, string pinnedElement)
        {
            var pinned =
                references.Keys.FirstOrDefault(p => p.Equals(pinnedElement, StringComparison.CurrentCultureIgnoreCase));

            if (pinned == null)
            {
                if (pinnedElement.Contains('-'))
                {
                    pinned = references.Keys.FirstOrDefault(p => p.StartsWith(pinnedElement, StringComparison.CurrentCultureIgnoreCase));
                    pinned = references.SelectMany(r => r.Value).FirstOrDefault(r => r.StartsWith(pinnedElement, StringComparison.CurrentCultureIgnoreCase));
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