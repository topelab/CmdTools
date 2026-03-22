namespace RelationsShared.Services
{
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    internal class MermaidFactory : IMermaidFactory
    {
        public string Create(IEnumerable<MermaidRelation> relations, Theme theme, Layout layout, Direction direction, string pinnedElement = null)
        {
            var contentBag = new StringBuilder();
            relations.ToList().ForEach(r => contentBag.AppendLine(r.ToString()));
            var content = contentBag.ToString();

            if (!string.IsNullOrEmpty(pinnedElement))
            {
                content = content.Replace($"\t{pinnedElement} ", $"\t{pinnedElement}:::pinned");
                content = content.Replace($":::pkg:::pinned", $":::pinnedpkg");
            }

            return GetComposition(content, theme, layout, direction);
        }

        protected string GetComposition(string content, Theme theme, Layout layout, Direction direction)
        {
            return $"""
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

        public string Render(string mmd, UserSettings userSettings)
        {
            var color = userSettings.HasBackgroundColor ? userSettings.BackgroundColor.ToLower() : "black";
            var encoded = System.Net.WebUtility.HtmlEncode(mmd);
            var html = $$"""
                <!doctype html>
                <html>
                <head>
                	<meta charset="utf-8">
                	<script src="https://unpkg.com/@panzoom/panzoom@4.6.1/dist/panzoom.min.js"></script>
                    <script type="module">
                		import mermaid from "https://cdn.jsdelivr.net/npm/mermaid@latest/dist/mermaid.esm.min.mjs";
                		import elkLayouts from "https://cdn.jsdelivr.net/npm/@mermaid-js/layout-elk@latest/dist/mermaid-layout-elk.esm.min.mjs";

                		// Registra el motor ELK con Mermaid
                		mermaid.registerLayoutLoaders(elkLayouts);

                		// Inicializa Mermaid
                		mermaid.initialize({
                			startOnLoad: false,
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
