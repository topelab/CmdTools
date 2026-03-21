namespace RelationsShared.Services
{
    using CmdTools.Contracts;
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using Topelab.Core.Resolver.Interfaces;

    internal class ProjectRelationsOpener(IUserSettingsFactory userSettingsFactory, IResolver resolver) : IProjectRelationsOpener
    {
        private UserSettings userSettings;
        private readonly IUserSettingsFactory userSettingsFactory = userSettingsFactory;
        private readonly IResolver resolver = resolver;

        private UserSettings UserSettings => userSettings ??= userSettingsFactory.Create();

        public async Task<RelationsUserControlContext> OpenAsync(RelationType relationType, SolutionExplorerItem solutionExplorerItem)
        {
            var projectOptions = BuildOptions(solutionExplorerItem, relationType);
            await RunAsync(projectOptions);
            var relationsWindowsContext = new RelationsUserControlContext
            {
                MermaidFile = projectOptions.OutputFile,
                UserSettings = UserSettings,
                Title = "Project relations",
                UserDataFolder = GetUserDataFolder(),
                Items = [.. solutionExplorerItem.Projects.Keys.OrderBy(k => k)],
                SelectedItem = solutionExplorerItem.Name,
                IsUsing = relationType == RelationType.Using,
                IsUsedBy = relationType == RelationType.UsedBy,
                IncludePackages = projectOptions.WithPackages
            };
            await GenerateAsync(relationsWindowsContext, projectOptions.OutputFile);
            relationsWindowsContext.PropertyChanged += (s, e) => OnRelationsWindowsContextPropertyChanged(s, e.PropertyName, solutionExplorerItem, relationsWindowsContext);
            return relationsWindowsContext;
        }

        private void OnRelationsWindowsContextPropertyChanged(object sender, string propertyName, SolutionExplorerItem solutionExplorerItem, RelationsUserControlContext relationsWindowsContext)
        {
            _ = OnRelationsWindowsContextPropertyChangedAsync(sender, propertyName, solutionExplorerItem, relationsWindowsContext);
        }

        private async Task OnRelationsWindowsContextPropertyChangedAsync(object sender, string propertyName, SolutionExplorerItem solutionExplorerItem, RelationsUserControlContext relationsWindowsContext)
        {
            if (sender is RelationsUserControlContext context)
            {
                switch (propertyName)
                {
                    case nameof(RelationsUserControlContext.RelationType):
                    case nameof(RelationsUserControlContext.SelectedItem):
                    case nameof(RelationsUserControlContext.IncludePackages):
                        var relationType = context.RelationType;
                        var localSolutionExplorerItem = solutionExplorerItem with { Name = context.SelectedItem, Path = Path.GetDirectoryName(solutionExplorerItem.Projects[context.SelectedItem]) };
                        var projectOptions = BuildOptions(localSolutionExplorerItem, relationType);
                        projectOptions.WithPackages = context.IncludePackages;
                        await RunAsync(projectOptions);
                        await GenerateAsync(context, projectOptions.OutputFile);
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
                PinnedProject = pinnedProject,
                OutputFile = outputFile,
                WithPackages = UserSettings.ShowPackages,
                Direction = Direction.LeftToRight,
                Theme = UserSettings.HasTheme ? Enum.Parse<Theme>(UserSettings.Theme) : Theme.Dark,
                Layout = Layout.Adaptive,
                Exclude = UserSettings.ExcludeProjects,
            };
        }

        private async Task RunAsync(ProjectOptions options)
        {
            var elementFinder = resolver.Get<IElementFinder>(options.FinderType.ToString());
            await Task.Run(() => elementFinder.Run(options));
        }

        public async Task GenerateAsync(RelationsUserControlContext relationsWindowsContext, string mmdFile)
        {
            var content = File.Exists(mmdFile) ? await File.ReadAllTextAsync(mmdFile) : "graph TD\n\tEmpty";
            var html = RenderMermaid(content);
            string fileName = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.html");
            await File.WriteAllTextAsync(fileName, html);
            relationsWindowsContext.Url = new Uri(fileName).AbsoluteUri;
        }

        private string RenderMermaid(string mmd)
        {
            var color = UserSettings.HasBackgroundColor ? UserSettings.BackgroundColor.ToLower() : "black";
            var encoded = System.Net.WebUtility.HtmlEncode(mmd);
            var html = $$"""
                <!doctype html>
                <html>
                <head>
                	<meta charset="utf-8">
                	<script src="https://unpkg.com/@panzoom/panzoom@4.6.0/dist/panzoom.min.js"></script>
                    <script type="module">
                		import mermaid from "https://cdn.jsdelivr.net/npm/mermaid@latest/dist/mermaid.esm.min.mjs";
                		import elkLayouts from "https://cdn.jsdelivr.net/npm/@mermaid-js/layout-elk@latest/dist/mermaid-layout-elk.esm.min.mjs";

                		// Registra el motor ELK con Mermaid
                		mermaid.registerLayoutLoaders(elkLayouts);

                		// Inicializa Mermaid
                		mermaid.initialize({
                			startOnLoad: false,
                			flowchart: { defaultRenderer: "elk" },
                            maxTextSize: {{UserSettings.MaxTextSize}},
                            maxEdges: {{UserSettings.MaxEdges}}
                		});

                		await mermaid.run({
                			querySelector: '.mermaid',
                			postRenderCallback: (id) => {
                				const container = document.getElementById("diagram-container");
                				const svgElement = container.querySelector("svg");

                				// Initialize Panzoom
                				const panzoomInstance = Panzoom(svgElement, {
                					maxScale: {{UserSettings.MaxZoomLevel.ToString("0.0", CultureInfo.InvariantCulture)}},
                					minScale: {{UserSettings.MinZoomLevel.ToString("0.0", CultureInfo.InvariantCulture)}},
                					step: 0.5,
                				});

                				// Add mouse wheel zoom
                				container.addEventListener("wheel", (event) => {
                					panzoomInstance.zoomWithWheel(event);
                				});
                			}
                		});
                	</script>
                	<style>
                		/* Estilos personalizados para Mermaid */
                        html, body {
                            height: 100%;
                            margin:0;
                            padding:0;
                            background-color: {{color}};
                        }
                		.mermaid {
                            height: 100vh; /* ocupa toda la altura de la ventana */
                            box-sizing: border-box;
                            background-color: {{color}};
                            padding: 0px;
                		}
                		.diagram-container {
                			width: 100%;
                			height: 100%;
                			overflow: hidden;
                			position: relative;
                		}
                		svg {
                			cursor: grab;
                		}
                	</style>
                	</head>
                	<body>
                		<div class="diagram-container" id="diagram-container">
                			<pre class="mermaid">{{encoded}}</pre>
                		</div>
                	</body>
                </html>
                """;

            return html;
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

        private string GetUserDataFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "WebView2");
        }
    }
}
