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
        [Option('v', "versions", Required = false, HelpText = "Set versions numbers")]
        public IEnumerable<string> Versions{ get; set; }

        [Option('p', "projects", Required = false, HelpText = "Set projects names to set versions")]
        public IEnumerable<string> Projects { get; set; }

        [Usage(ApplicationAlias = "UpdateVersion")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Normal scenario", new Options { Versions = new List<string>() { "1.0.0", "2.0.0"}, Projects = new List<string>() { "FirstProject", "SecondProject" } });
            }
        }
    }
}
