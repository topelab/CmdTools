using CommandLine;

namespace CreateRelationsDiagram
{
    internal class Options
    {
        [Option('p', "path", Required = false, HelpText = "Set solution path")]
        public string BasePath { get; set; }

        [Option('o', "output", Required = false, Default = Constants.RelationsFileName, HelpText = $"Output file name (default: {Constants.RelationsFileName})")]
        public string OutputFile { get; set; }

    }
}