namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using CommandLine;

    [Verb("classes", HelpText = "Manage class options for CreateRelationsDiagram.")]
    internal class ClassOptions : Options
    {
        [Option('a', "assembly", Required = false, HelpText = "Assembly (full path to dll) where classes will be processed")]
        public string Assembly { get; set; }

        [Option('n', "namespace", Required = false, HelpText = "NameSpace to process (if not set, all name spaces in the solution will be processed)")]
        public string NameSpace { get; set; }

        [Option('c', "class", Required = false, HelpText = "Class name to process (if not set, all classes in the assembly will be processed)")]
        public string ClassName { get; set; }

        public override FinderType FinderType => Reverse ? FinderType.ReverseClasses : FinderType.Classes;
    }
}
