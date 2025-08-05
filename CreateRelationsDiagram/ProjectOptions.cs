namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using CommandLine;

    [Verb("projects", isDefault: true, HelpText = "Manage project options for CreateRelationsDiagram.")]
    internal class ProjectOptions : Options
    {
        [Option('s', "root", Required = false, HelpText = "Set root path")]
        public string RootPath { get; set; }

        [Option('f', "filter", Required = false, HelpText = "Filter by project (if not set, all projects in the solution will be processed)")]
        public string ProjectFilter { get; set; }

        [Option('p', "depends-on", Required = false, HelpText = "Get projects that depends on project")]
        public string PinnedProject { get; set; }

        [Option('w', "with-packages", Required = false, Default = false, HelpText = "Nuget packages will be collected")]
        public bool WithPackages { get; set; }

        public override FinderType FinderType => Reverse ? FinderType.ReverseProjects : FinderType.Projects;
    }
}
