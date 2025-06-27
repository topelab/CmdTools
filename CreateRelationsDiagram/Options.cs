using CommandLine;

namespace CreateRelationsDiagram
{
    internal class Options
    {
        [Option('s', "solution", Required = false, HelpText = "Set solution path")]
        public string SolutionPath { get; set; }

        [Option('o', "output", Required = false, Default = Constants.RelationsFileName, HelpText = $"Output file name (default: {Constants.RelationsFileName})")]
        public string OutputFile { get; set; }

        [Option('p', "project", Required = false, HelpText = "Filter by project (if not set, all projects in the solution will be processed)")]
        public string ProjectPath { get; set; }

        [Option('e', "exclude", Required = false, HelpText = "Exclude specific projects from processing (comma-separated list)")]
        public IEnumerable<string> Exclude { get; set; }
    }
}