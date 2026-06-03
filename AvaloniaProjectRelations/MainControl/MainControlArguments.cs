namespace AvaloniaProjectRelations.MainControl
{
    using CommandLine;

    internal class MainControlArguments
    {
        [Option('s', "root", Required = false, HelpText = "Set root path")]
        public string RootPath { get; set; }

        [Option('p', "project", Required = false, HelpText = "Set selected project")]
        public string SelectedProject { get; set; }

        [Option('e', "exclude", Required = false, HelpText = "Exclude specific elements from processing (regular expression)")]
        public string Exclude { get; set; }

        [Option('w', "with-packages", Required = false, HelpText = "Nuget packages will be collected")]
        public bool? WithPackages { get; set; }

        [Option('u', "is-used-by", Required = false, Default = false, HelpText = "Show projects that are used by the selected project, otherwise show projects that use the selected project")]
        public bool IsUsedBy { get; set; }

        [Option('d', "show-diagram", Required = false, Default = false, HelpText = "Show diagram only")]
        public bool ShowDiagram { get; set; }

    }
}
