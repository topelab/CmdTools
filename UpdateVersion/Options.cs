using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace UpdateVersion
{
    /// <summary>
    /// Arguments options
    /// </summary>
    internal class Options
    {
        [Option('p', "path", Required = true, HelpText = "Set solution path")]
        public string BasePath { get; set; }

        [Option('v', "versions", Required = false, HelpText = "Set versions numbers on matched projects names", SetName = "inLine")]
        public IEnumerable<string> Versions { get; set; }

        [Option('b', "bump-version-level", Default = null, Required = false, HelpText = "Bump version level on pattern entries found at version file with format \"[*pattern*|ALL: ](1|2|3)\"", SetName = "file")]
        public IEnumerable<string> VersionsToBump { get; set; }

        [Option("bump-major", HelpText = "Update versions on projects", SetName = "file")]
        public bool BumpMajorVersion { get; set; }
        [Option("bump-minor", HelpText = "Update versions on projects", SetName = "file")]
        public bool BumpMinorVersion { get; set; }
        [Option("bump-patch", HelpText = "Update versions on projects", SetName = "file")]
        public bool BumpPatchVersion { get; set; }


        [Option('u', "update", Default = (bool)true, HelpText = "Update versions on projects")]
        public bool? Update { get; set; }


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

        public Options Resolve()
        {
            if (BumpPatchVersion || BumpMinorVersion || BumpMajorVersion)
            {
                var versionLevel = BumpPatchVersion ? 3 : BumpMinorVersion ? 2 : 1;
                VersionsToBump = new List<string>() { $"{Constants.BumpAllProjects}{Constants.ProjectVersionSeparator} {versionLevel}" };
            }

            return this;
        }
    }
}
