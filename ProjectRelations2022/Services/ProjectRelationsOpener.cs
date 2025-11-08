namespace ProjectRelations2022.Services
{
    using CmdTools.Contracts;
    using CreateRelationsDiagram;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Internal.VisualStudio.Extensibility.Framework;
    using ProjectRelations2022.DTO;
    using ProjectRelations2022.Views;
    using System.IO;
    using System.Threading.Tasks;
    using Topelab.Core.Resolver.Interfaces;

    internal class ProjectRelationsOpener : IProjectRelationsOpener
    {
        private readonly UserSettings userSettings;

        public ProjectRelationsOpener(IUserSettingsFactory userSettingsFactory)
        {
            userSettings = userSettingsFactory.Create();
            //var envPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "WebView2");
            //System.Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", envPath);
            //System.Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--allow-file-access-from-files");
        }

        public async Task<RelationsUserControlContext> OpenUsedByProjectAsync(string projectPath, string projectName)
        {
            var outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", $"used-by-{projectName.ToLower()}.mmd");
            var projectOptions = BuildOptions(projectPath, null, null, outputFile);
            await RunAsync(projectOptions);
            var relationsWindowsContext = new RelationsUserControlContext
            {
                MermaidFile = outputFile,
                UserSettings = userSettings,
                Title = $"Projects USED BY {projectName}",
                UserDataFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "WebView2")
            };
            await GenerateAsync(relationsWindowsContext, outputFile);
            return relationsWindowsContext;
        }

        public async Task<RelationsUserControlContext> OpenUsingProjectAsync(string solutionPah, string projectName)
        {
            var outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", $"using-{projectName.ToLower()}.mmd");
            var projectOptions = BuildOptions(solutionPah, null, projectName, outputFile);
            await RunAsync(projectOptions);
            var relationsWindowsContext = new RelationsUserControlContext
            {
                MermaidFile = outputFile,
                UserSettings = userSettings,
                Title = $"Projects USING {projectName}",
                UserDataFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "WebView2")
            };
            await GenerateAsync(relationsWindowsContext, outputFile);
            return relationsWindowsContext;
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

        private async Task RunAsync(ProjectOptions options)
        {
            var resolver = ExtensionContext.ServiceProvider.GetService<IResolver>();
            var elementFinder = resolver.Get<IElementFinder>(options.FinderType.ToString());
            await Task.Run(() => elementFinder.Run(options));
        }

        public async Task GenerateAsync(RelationsUserControlContext relationsWindowsContext, string mmdFile)
        {
            var content = File.Exists(mmdFile) ? await File.ReadAllTextAsync(mmdFile) : "graph TD\n\tEmpty";
            var html = RenderMermaid(content);
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "result.html");
            await File.WriteAllTextAsync(fileName, html);
            relationsWindowsContext.Url = new Uri(fileName).AbsoluteUri;
        }

        private string RenderMermaid(string mmd)
        {
            var color = userSettings.HasBackgroundColor ? userSettings.BackgroundColor.ToLower() : "black";
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
                			flowchart: { defaultRenderer: "elk" }
                		});

                		await mermaid.run({
                			querySelector: '.mermaid',
                			postRenderCallback: (id) => {
                				const container = document.getElementById("diagram-container");
                				const svgElement = container.querySelector("svg");

                				// Initialize Panzoom
                				const panzoomInstance = Panzoom(svgElement, {
                					maxScale: 5,
                					minScale: 0.5,
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

    }
}
