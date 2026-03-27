namespace RelationsShared.Services
{
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class TextRender : IOutputRender
    {
        public string Create(IEnumerable<Relation> relations, Options options, string pinnedElement = null)
        {
            var contentBag = new StringBuilder();
            relations.Select(r => r.Element)
                .Union(relations.Select(r => r.Reference))
                .Distinct()
                .OrderBy(e => e)
                .ToList()
                .ForEach(r =>
                {
                    bool isPinned = pinnedElement != null && r == pinnedElement;
                    bool isPkg = r.EndsWith(":::pkg");
                    r = r.Replace(":::pkg", "");
                    var clasess = GetClasses(isPinned, isPkg);
                    contentBag.AppendLine($"<li{clasess}>{r}</li>");
                });

            return contentBag.ToString();
        }

        private static string GetClasses(bool isPinned, bool isPkg)
        {
            List<string> clasess = [];
            if (isPkg)
            {
                clasess.Add("pkg");
            }
            if (isPinned)
            {
                clasess.Add("pinned");
            }

            return clasess.Count > 0 ? $" class=\"{string.Join(" ", clasess)}\"" : "";
        }

        public string Render(string input, UserSettings userSettings)
        {
            var color = userSettings.HasBackgroundColor ? userSettings.BackgroundColor.ToLower() : "black";
            var html = $$"""
                <!doctype html>
                <html>
                <head>
                	<meta charset="utf-8">
                	<style>
                		/* Estilos personalizados */
                        html, body {
                            height: 100%;
                            margin:0;
                            padding:0;
                            font-family: Verdana, Geneva, Tahoma, sans-serif;
                            font-size: 10pt;
                            background-color: {{color}};
                        div.text-container {
                            color: {{color}};
                            filter: invert(100%);
                        }
                        ul {
                            display: flex;
                            flex-direction: column;
                            gap: 3px;
                        }
                        li.pkg::after { content: '📦'; margin-left: 10px; filter: invert(100%); }
                        li.pinned::after { content: '📌'; margin-left: 10px; filter: invert(100%); }
                	</style>
                	</head>
                	<body>
                		<div class="text-container">
                            <ul>
                                {{input}}
                            </ul>
                		</div>
                	</body>
                </html>
                """;

            return html;
        }
    }
}
