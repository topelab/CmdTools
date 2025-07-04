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

        [Option('p', "project", Required = false, HelpText = "Filter by project (if not set, all projects in the solution will be processed)", SetName = ProjectsGroup)]
        public string ProjectFilter { get; set; }

        [Option('n', "namespace", Required = false, HelpText = "NameSpace to process (if not set, all name spaces in the solution will be processed)", SetName = ClassesGroup)]
        public string NameSpace { get; set; }

        [Option('a', "assembly", Required = false, HelpText = "Assembly (full path to dll) where classes will be processed", SetName = ClassesGroup)]
        public string Assembly { get; set; }

        [Option('c', "class", Required = false, HelpText = "Class name to process (if not set, all classes in the assembly will be processed)", SetName = ClassesGroup)]
        public string ClassName { get; set; }

        [Option('o', "output", Required = false, Default = Constants.RelationsFileName, HelpText = $"Output file name (default: {Constants.RelationsFileName})")]
        public string OutputFile { get; set; }

        [Option('e', "exclude", Required = false, HelpText = "Exclude specific elements from processing (comma-separated list)")]
        public IEnumerable<string> Exclude { get; set; }

        [Option('r', "reverse", Required = false, Default = false, HelpText = "Reverse the direction of the relations in the diagram")]
        public bool Reverse { get; set; }

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