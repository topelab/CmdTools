namespace RelationsShared.Finders
{
    internal class ElementFinderBase
    {
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