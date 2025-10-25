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
                classDef pinned stroke:orange, stroke-width:2px, stroke-dasharray: 3 2;
                classDef pinnedpkg fill:#658, stroke:orange, stroke-width:2px, stroke-dasharray: 3 2;
                """;
        }

        protected void Finalize(string content, string outputFile, bool openOutput)
        {
            if (string.IsNullOrEmpty(outputFile))
            {
                Console.WriteLine(content);
            }
            else
            {
                File.WriteAllText(outputFile, content);
                Console.WriteLine($"Diagram created at: {outputFile}");
                if (openOutput)
                {
                    try
                    {
                        var process = new System.Diagnostics.Process();
                        process.StartInfo = new System.Diagnostics.ProcessStartInfo(outputFile)
                        {
                            UseShellExecute = true
                        };
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to open the output file: {ex.Message}");
                    }
                }
            }
        }
    }
}