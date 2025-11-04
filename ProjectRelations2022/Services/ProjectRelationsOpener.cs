namespace ProjectRelations2022.Services
{
    using CmdTools.Contracts;
    using CreateRelationsDiagram;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Internal.VisualStudio.Extensibility.Framework;
    using Microsoft.VisualStudio.Threading;
    using ProjectRelations2022.DTO;
    using System.IO;
    using System.Threading.Tasks;
    using Topelab.Core.Resolver.Interfaces;

    internal class ProjectRelationsOpener : IProjectRelationsOpener
    {
        private readonly UserSettings userSettings;

        public ProjectRelationsOpener(IUserSettingsFactory userSettingsFactory)
        {
            userSettings = userSettingsFactory.Create();
        }

        public async Task OpenUsedByProjectAsync(string projectPath, string projectName)
        {
            var outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", $"used-by-{projectName.ToLower()}.mmd");
            var projectOptions = BuildOptions(Path.GetDirectoryName(projectPath), null, null, outputFile);
            await RunAsync($"Projects USED BY {projectName}", projectOptions);
        }

        public async Task OpenUsingProjectAsync(string solutionPah, string projectName)
        {
            var outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", $"using-{projectName.ToLower()}.mmd");
            var projectOptions = BuildOptions(Path.GetDirectoryName(solutionPah), null, projectName, outputFile);
            await RunAsync($"Projects USING {projectName}", projectOptions);
        }

        private ProjectOptions BuildOptions(string rootPath, string projectFilter, string pinnedProject, string outputFile)
        {
            return new ProjectOptions
            {
                RootPath = rootPath,
                ProjectFilter = projectFilter,
                PinnedProject = pinnedProject,
                OutputFile = outputFile,
                WithPackages = userSettings.ShowPackages,
                Direction = Direction.LeftToRight,
                Theme = userSettings.HasTheme ? Enum.Parse<Theme>(userSettings.Theme) : Theme.Dark,
                Layout = Layout.Adaptive
            };
        }

        private async Task RunAsync(string title, ProjectOptions options)
        {
            var resolver = ExtensionContext.ServiceProvider.GetService<IResolver>();
            var elementFinder = resolver.Get<IElementFinder>(options.FinderType.ToString());
            elementFinder.Run(options);
            var joinableTaskContext = ExtensionContext.ServiceProvider.GetService<JoinableTaskContext>();

            await joinableTaskContext.Factory.SwitchToMainThreadAsync();

        }
    }
}
