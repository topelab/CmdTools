namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using CommandLine;

    internal class Options
    {
        private const string ProjectsGroup = "projects";
        private const string ClassesGroup = "classes";

        [Option('s', "solution", Required = false, HelpText = "Set solution path", SetName = ProjectsGroup)]
        public string SolutionPath { get; set; }

        [Option('f', "filter", Required = false, HelpText = "Filter by project (if not set, all projects in the solution will be processed)", SetName = ProjectsGroup)]
        public string ProjectFilter { get; set; }

        [Option('w', "WithPackages", Required = false, Default = false, HelpText = "Nuget packages will be collected", SetName = ProjectsGroup)]
        public bool WithPackages { get; set; }

        [Option('p', "pinned", Required = false, HelpText = "Show all projects that use or is used by pinned project", SetName = ProjectsGroup)]
        public string PinnedProject { get; set; }


        [Option('a', "assembly", Required = false, HelpText = "Assembly (full path to dll) where classes will be processed", SetName = ClassesGroup)]
        public string Assembly { get; set; }

        [Option('n', "namespace", Required = false, HelpText = "NameSpace to process (if not set, all name spaces in the solution will be processed)", SetName = ClassesGroup)]
        public string NameSpace { get; set; }

        [Option('c', "class", Required = false, HelpText = "Class name to process (if not set, all classes in the assembly will be processed)", SetName = ClassesGroup)]
        public string ClassName { get; set; }

        [Option('o', "output", Required = false, Default = Constants.RelationsFileName, HelpText = $"Output file name (default: {Constants.RelationsFileName})")]
        public string OutputFile { get; set; }

        [Option('e', "exclude", Required = false, HelpText = "Exclude specific elements from processing (comma-separated list)")]
        public IEnumerable<string> Exclude { get; set; }

        [Option('r', "reverse", Required = false, Default = false, HelpText = "Reverse the direction of the relations in the diagram")]
        public bool Reverse { get; set; }

        [Option('d', "direction", Required = false, Default = Direction.TopToDown, HelpText = "Direction of the diagram (TopToDown, LefToRight, RightToLeft or BottomToTop; default: TopToDown)")]
        public Direction Direction { get; set; }

        [Option('t', "theme", Required = false, Default = Theme.NeoDark, HelpText = "Theme of the diagram (Default, Base, MermaidChart, Neo, NeoDark, Forest, Dark or Neutral; default: NeoDark)")]
        public Theme Theme { get; set; }

        [Option('l', "layout", Required = false, Default = Layout.Adaptive, HelpText = "Layout of the diagram (Hierarchical or Adaptative; default: Adaptative)")]
        public Layout Layout { get; set; }

        public FinderType FinderType
        {
            get
            {
                if (!string.IsNullOrEmpty(Assembly))
                {
                    return Reverse ? FinderType.ReverseClasses : FinderType.Classes;
                }
                else
                {
                    return Reverse ? FinderType.ReverseProjects : FinderType.Projects;
                }
            }
        }
    }
}