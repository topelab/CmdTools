using RelationsShared.Services;

namespace ProjectRelations2026.Services
{
    using CmdTools.Shared;
    using ProjectRelations2026.Views;
    using RelationsShared.DTO;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Topelab.Core.Resolver.Interfaces;

    internal class RelationsContextFactory(IUserSettingsFactory userSettingsFactory, IResolver resolver, IOutputRenderFactory outputRenderFactory) : IRelationsContextFactory
    {
        private UserSettings userSettings;
        private readonly IUserSettingsFactory userSettingsFactory = userSettingsFactory;
        private readonly IResolver resolver = resolver;
        private readonly IOutputRenderFactory outputRenderFactory = outputRenderFactory;

        private UserSettings UserSettings => userSettings ??= userSettingsFactory.Create(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

        public async Task<RelationsContext> CreateAsync(RelationType relationType, SolutionExplorerItem solutionExplorerItem)
        {
            var projectOptions = BuildOptions(solutionExplorerItem, relationType);
            var content = await RunAsync(projectOptions);
            var relationsWindowsContext = new RelationsContext
            {
                MermaidFile = projectOptions.OutputFile,
                UserSettings = UserSettings,
                Title = "Project relations",
                Items = [.. GetFilteredProjects(solutionExplorerItem, projectOptions.Exclude)],
                SelectedItem = solutionExplorerItem.Name,
                IsUsing = relationType == RelationType.Using,
                IsUsedBy = relationType == RelationType.UsedBy,
                IncludePackages = projectOptions.WithPackages,
                ShowListOnly = projectOptions.RenderType == RenderType.Text,
            };
            await GenerateAsync(relationsWindowsContext, content);
            relationsWindowsContext.PropertyChanged += (s, e) => OnRelationsWindowsContextPropertyChanged(s, e.PropertyName, solutionExplorerItem, relationsWindowsContext);
            return relationsWindowsContext;
        }

        private IEnumerable<string> GetFilteredProjects(SolutionExplorerItem solutionExplorerItem, string excludeProjectsOption)
        {
            var excludeProjects = string.IsNullOrEmpty(excludeProjectsOption) ? null : new Regex(excludeProjectsOption, RegexOptions.IgnoreCase);
            return solutionExplorerItem.Projects.Keys.Where(k => excludeProjects == null || !excludeProjects.IsMatch(k)).OrderBy(k => k);
        }

        private void OnRelationsWindowsContextPropertyChanged(object sender, string propertyName, SolutionExplorerItem solutionExplorerItem, RelationsContext relationsWindowsContext)
        {
            _ = OnRelationsWindowsContextPropertyChangedAsync(sender, propertyName, solutionExplorerItem, relationsWindowsContext);
        }

        private async Task OnRelationsWindowsContextPropertyChangedAsync(object sender, string propertyName, SolutionExplorerItem solutionExplorerItem, RelationsContext relationsWindowsContext)
        {
            if (sender is RelationsContext context)
            {
                switch (propertyName)
                {
                    case nameof(RelationsContext.RelationType):
                    case nameof(RelationsContext.SelectedItem):
                    case nameof(RelationsContext.IncludePackages):
                    case nameof(RelationsContext.ShowListOnly):
                        var relationType = context.RelationType;
                        var localSolutionExplorerItem = solutionExplorerItem with { Name = context.SelectedItem, Path = Path.GetDirectoryName(solutionExplorerItem.Projects[context.SelectedItem]) };
                        var projectOptions = BuildOptions(localSolutionExplorerItem, relationType);
                        projectOptions.WithPackages = context.IncludePackages;
                        projectOptions.RenderType = context.ShowListOnly ? RenderType.Text : RenderType.Mermaid;
                        var content = await RunAsync(projectOptions);
                        await GenerateAsync(context, content);
                        break;
                }
            }
        }

        private ProjectOptions BuildOptions(SolutionExplorerItem solutionExplorerItem, RelationType relationType)
        {
            var rootPath = GetRootPath(relationType, solutionExplorerItem);
            var pinnedProject = GetPinnedProject(relationType, solutionExplorerItem);
            var outputFile = GetOutputFile(relationType, solutionExplorerItem.Name);

            return new ProjectOptions
            {
                RootPath = rootPath,
                PinnedElement = pinnedProject,
                OutputFile = outputFile,
                WithPackages = UserSettings.ShowPackages,
                Direction = Direction.LeftToRight,
                Theme = UserSettings.HasTheme ? Enum.Parse<Theme>(UserSettings.Theme) : Theme.Dark,
                Layout = Layout.Adaptive,
                Exclude = UserSettings.ExcludeProjects,
                RenderType = RenderType.Text,
                SelectedElement = solutionExplorerItem.Name,
            };
        }

        private async Task<string> RunAsync(ProjectOptions options)
        {
            var elementRelationsGetter = resolver.Get<IElementRelationsGetter>(options.FinderType.ToString());
            var relations = elementRelationsGetter.Get(options);
            var outputRender = outputRenderFactory.Create(options.RenderType);
            return await Task.Run(() => outputRender.Create(relations, options));
        }

        public async Task GenerateAsync(RelationsContext relationsWindowsContext, string content)
        {
            var outputRender = outputRenderFactory.Create(relationsWindowsContext.ShowListOnly ? RenderType.Text : RenderType.Mermaid);
            var html = outputRender.RenderToHtml(content, UserSettings);
            string fileName = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.html");
            await File.WriteAllTextAsync(fileName, html);
            relationsWindowsContext.Url = new Uri(fileName).AbsoluteUri;
        }

        private string GetRootPath(RelationType relationType, SolutionExplorerItem solutionExplorerItem)
        {
            return relationType switch
            {
                RelationType.Using => solutionExplorerItem.SolutionPath,
                RelationType.UsedBy => solutionExplorerItem.Path,
                _ => throw new NotImplementedException(),
            };
        }

        private string GetPinnedProject(RelationType relationType, SolutionExplorerItem solutionExplorerItem)
        {
            return relationType switch
            {
                RelationType.Using => solutionExplorerItem.Name,
                RelationType.UsedBy => null,
                _ => throw new NotImplementedException(),
            };
        }

        private string GetOutputFile(RelationType relationType, string projectName)
        {
            var prefix = relationType.GetDescription().ToLower().Replace(" ", "-");
            return Path.Combine(Path.GetTempPath(), $"{prefix}-{projectName.ToLower()}-{Guid.NewGuid()}.mmd");
        }
    }
}
