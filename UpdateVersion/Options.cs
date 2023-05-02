using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace UpdateVersion
{
    /// <summary>
    /// Args options
    /// </summary>
    internal class Options
    {
        [Option('p', "path", Required = false, HelpText = "Set solution path")]
        public string BasePath { get; set; }

        [Option('v', "versions", Required = false, HelpText = "Set versions numbers on matched projects names", SetName = "cmdline")]
        public IEnumerable<string> Versions{ get; set; }

        [Option('b', "bump-version", Default = null, Required = false, HelpText = "Bump version level on pattern entries found at version file with format \"*pattern*|ALL: (1|2|3)\"", SetName = "file")]
        public IEnumerable<string> VersionsToBump { get; set; }


        [Option('f', "versions-file", Required = false, Default = "version.txt", HelpText = "File where versions are defined", SetName = "file")]
        public string VersionsFile { get; set; }


        [Usage(ApplicationAlias = "UpdateVersion")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Normal scenario", new Options { Versions = new List<string>() { "1.0.0", "SecondProject: 2.0.0" } });
            }
        }
    }
}
