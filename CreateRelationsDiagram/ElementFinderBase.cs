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
    }
}