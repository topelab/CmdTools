namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using CommandLine;

    internal abstract class Options
    {
        [Option('o', "output", Required = false, HelpText = $"Output file name (default: output to console)")]
        public string OutputFile { get; set; }

        [Option('e', "exclude", Required = false, HelpText = "Exclude specific elements from processing (regular expression)")]
        public string Exclude { get; set; }

        [Option('r', "reverse", Required = false, Default = false, HelpText = "Reverse the direction of the relations in the diagram")]
        public bool Reverse { get; set; }

        [Option('d', "direction", Required = false, Default = Direction.TopToDown, HelpText = "Direction of the diagram (TopToDown, LefToRight, RightToLeft or BottomToTop; default: TopToDown)")]
        public Direction Direction { get; set; }

        [Option('t', "theme", Required = false, Default = Theme.NeoDark, HelpText = "Theme of the diagram (Default, Base, MermaidChart, Neo, NeoDark, Forest, Dark or Neutral; default: NeoDark)")]
        public Theme Theme { get; set; }

        [Option('l', "layout", Required = false, Default = Layout.Adaptive, HelpText = "Layout of the diagram (Hierarchical or Adaptative; default: Adaptative)")]
        public Layout Layout { get; set; }

        public abstract FinderType FinderType { get; }
    }
}