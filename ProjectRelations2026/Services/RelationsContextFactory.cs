using RelationsShared.Services;

namespace ProjectRelations2026.Services
{
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal class RelationsContextFactory(IUserSettingsFactory userSettingsFactory,
                                           IOutputRenderFactory outputRenderFactory,
                                           IProjectRelationsContextInitializer projectRelationsContextInitializer,
                                           IElementRelationsGetterFactory elementRelationsGetterFactory) : IRelationsContextFactory
    {
        private UserSettings userSettings;
        private readonly IUserSettingsFactory userSettingsFactory = userSettingsFactory;
        private readonly IOutputRenderFactory outputRenderFactory = outputRenderFactory;
        private readonly IProjectRelationsContextInitializer projectRelationsContextInitializer = projectRelationsContextInitializer;
        private readonly IElementRelationsGetterFactory elementRelationsGetterFactory = elementRelationsGetterFactory;
        private ProjectRelationsContext context;

        private UserSettings UserSettings => userSettings ??= userSettingsFactory.Create(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

        public async Task<Views.RelationsContext> CreateAsync(RelationType relationType, SolutionExplorerItem solutionExplorerItem)
        {
            var projectOptions = BuildOptions(solutionExplorerItem, relationType);
            context = new() { Options = projectOptions };
            projectRelationsContextInitializer.Initialize(context);

            var relationsWindowsContext = new Views.RelationsContext
            {
                MermaidFile = projectOptions.OutputFile,
                UserSettings = UserSettings,
                Title = $"Project relations (v. {GetVersion()})",
                Items = [.. GetFilteredProjects(solutionExplorerItem, projectOptions.Exclude)],
                SelectedItem = solutionExplorerItem.Name,
                UsingMe = relationType == RelationType.Using,
                UsedByMe = relationType == RelationType.UsedBy,
                IncludePackages = projectOptions.WithPackages,
                ShowListOnly = projectOptions.RenderType == RenderType.Text,
            };

            relationsWindowsContext.PropertyChanged += (s, e) => OnRelationsWindowsContextPropertyChanged(s, e.PropertyName, solutionExplorerItem);
            await GenerateAsync(relationsWindowsContext, projectOptions);

            return relationsWindowsContext;
        }

        private IEnumerable<string> GetFilteredProjects(SolutionExplorerItem solutionExplorerItem, string excludeProjectsOption)
        {
            var excludeProjects = string.IsNullOrEmpty(excludeProjectsOption) ? null : new Regex(excludeProjectsOption, RegexOptions.IgnoreCase);
            return solutionExplorerItem.Projects.Keys.Where(k => excludeProjects == null || !excludeProjects.IsMatch(k)).OrderBy(k => k);
        }

        private void OnRelationsWindowsContextPropertyChanged(object sender, string propertyName, SolutionExplorerItem solutionExplorerItem)
        {
            _ = OnRelationsWindowsContextPropertyChangedAsync(sender, propertyName, solutionExplorerItem);
        }

        private async Task OnRelationsWindowsContextPropertyChangedAsync(object sender, string propertyName, SolutionExplorerItem solutionExplorerItem)
        {
            if (sender is Views.RelationsContext relationsWindowsContext)
            {
                var options = context.Options as ProjectOptions;

                switch (propertyName)
                {
                    case nameof(Views.RelationsContext.RelationType):
                    case nameof(Views.RelationsContext.SelectedItem):
                    case nameof(Views.RelationsContext.IncludePackages):
                    case nameof(Views.RelationsContext.ShowListOnly):
                        if (relationsWindowsContext.SelectedItem != null)
                        {
                            var relationType = relationsWindowsContext.RelationType;
                            options.RootPath = GetRootPath(relationType, solutionExplorerItem);
                            options.PinnedElement = GetPinnedProject(relationType, relationsWindowsContext.SelectedItem);
                            options.SelectedElement = relationsWindowsContext.SelectedItem;
                            options.WithPackages = relationsWindowsContext.IncludePackages;
                            options.RenderType = relationsWindowsContext.ShowListOnly ? RenderType.Text : RenderType.Mermaid;

                            await GenerateAsync(relationsWindowsContext, options);
                        }

                        break;
                }
            }
        }

        private ProjectOptions BuildOptions(SolutionExplorerItem solutionExplorerItem, RelationType relationType)
        {
            var pinnedProject = GetPinnedProject(relationType, solutionExplorerItem.Name);
            var outputFile = GetOutputFile(relationType, solutionExplorerItem.Name);

            return new ProjectOptions
            {
                InitialPath = solutionExplorerItem.SolutionPath,
                RootPath = solutionExplorerItem.SolutionPath,
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

        public async Task GenerateAsync(Views.RelationsContext relationsWindowsContext, ProjectOptions options)
        {
            var elementRelationsGetter = elementRelationsGetterFactory.Create(options.FinderType);
            var relations = elementRelationsGetter.GetFromContext(context);
            var outputRender = outputRenderFactory.Create(options.RenderType);
            var content = outputRender.Create(relations, options);
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

        private string GetPinnedProject(RelationType relationType, string selectedElement)
        {
            return relationType switch
            {
                RelationType.Using => selectedElement,
                RelationType.UsedBy => null,
                _ => throw new NotImplementedException(),
            };
        }

        private string GetOutputFile(RelationType relationType, string projectName)
        {
            var prefix = relationType.GetDescription().ToLower().Replace(" ", "-");
            return Path.Combine(Path.GetTempPath(), $"{prefix}-{projectName.ToLower()}-{Guid.NewGuid()}.mmd");
        }

        private static string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}
