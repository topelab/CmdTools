namespace RelationsShared.Services
{
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    internal class MermaidRender : IOutputRender
    {
        public string Create(IEnumerable<Relation> relations, Options options)
        {
            var contentBag = new StringBuilder();
            string content = string.Empty;

            var pinnedElement = options.PinnedElement;
            var rootElement = options.SelectedElement;

            if (!string.IsNullOrEmpty(pinnedElement))
            {
                relations.ToList().ForEach(r => contentBag.AppendLine($"\t{r.Reference} -->\t{r.Element} "));
                content = contentBag.ToString();
                content = content.Replace($"\t{pinnedElement} ", $"\t{pinnedElement}:::pinned");
                content = content.Replace($":::pkg:::pinned", $":::pinnedpkg");
            }
            else
            {
                relations.ToList().ForEach(r => contentBag.AppendLine($"\t{r.Element} -->\t{r.Reference} "));
                content = contentBag.ToString();
            }

            return GetComposition(content, options.Theme, options.Layout, options.Direction);
        }


        protected string GetComposition(string content, Theme theme, Layout layout, Direction direction)
        {
            if (string.IsNullOrEmpty(content))
            {
                content = "graph TD\n\tEmpty";
            }
            else
            {
                content = $"""
                    ---
                    config:
                      theme: {theme.GetDescription()}
                      layout: {layout.GetDescription()}
                    ---
                    flowchart {direction.GetDescription()}
                    {content}

                    classDef pkg fill:#658;
                    classDef pinned stroke:orange, stroke-width:2px, stroke-dasharray: 3 2;
                    classDef pinnedpkg fill:#658, stroke:orange, stroke-width:2px, stroke-dasharray: 3 2;
                    """;
            }

            return content;
        }

        public string RenderToHtml(string input, UserSettings userSettings)
        {
            var color = userSettings.HasBackgroundColor ? userSettings.BackgroundColor.ToLower() : "black";
            var encoded = System.Net.WebUtility.HtmlEncode(input);
            var html = $$"""
                <!doctype html>
                <html>
                <head>
                	<meta charset="utf-8">
                	<script src="{{userSettings.BaseUrl}}scripts/panzoom.min.js"></script>
                    <script type="module">
                		import mermaid from "{{userSettings.BaseUrl}}scripts/mermaid.esm.min.mjs";
                		import elkLayouts from "{{userSettings.BaseUrl}}scripts/mermaid-layout-elk.esm.min.mjs";

                		// Registra el motor ELK con Mermaid
                		mermaid.registerLayoutLoaders(elkLayouts);

                		// Inicializa Mermaid
                		mermaid.initialize({
                			startOnLoad: true,
                			flowchart: { defaultRenderer: "elk" },
                            maxTextSize: {{userSettings.MaxTextSize}},
                            maxEdges: {{userSettings.MaxEdges}}
                		});

                		await mermaid.run({
                			querySelector: '.mermaid',
                			postRenderCallback: (id) => {
                				const container = document.getElementById("diagram-container");
                				const svgElement = container.querySelector("svg");

                				// Initialize Panzoom
                				const panzoomInstance = Panzoom(svgElement, {
                					maxScale: {{userSettings.MaxZoomLevel.ToString("0.0", CultureInfo.InvariantCulture)}},
                					minScale: {{userSettings.MinZoomLevel.ToString("0.0", CultureInfo.InvariantCulture)}},
                					step: 0.5,
                				});

                				// Add mouse wheel zoom
                				container.addEventListener("wheel", (event) => {
                					panzoomInstance.zoomWithWheel(event);
                				});

                                container.style.top = "0px";
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
                			height: 98%;
                			overflow: hidden;
                			position: relative;
                            top: -10000px;
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
