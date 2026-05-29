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
        public string Create(IEnumerable<Relation> relations, Options options)
        {
            var contentBag = new StringBuilder();
            var pinnedElement = options.PinnedElement;
            var rootElement = options.SelectedElement;

            if (!string.IsNullOrEmpty(pinnedElement))
            {
                relations.Select(r => r.Element)
                    .Union(relations.Select(r => r.Reference))
                    .Distinct()
                    .Where(e => e != pinnedElement)
                    .OrderBy(e => e)
                    .ToList()
                    .ForEach(r =>
                    {
                        bool isPkg = r.EndsWith(":::pkg");
                        r = r.Replace(":::pkg", "");
                        var clasess = GetClasses(false, isPkg);
                        contentBag.AppendLine($"<li{clasess}>{r}</li>");
                    });
            }
            else
            {
                GetRelationsWithLevels(rootElement, relations)
                    .OrderBy(r => r.Level)
                    .ThenBy(r => r.Element)
                    .ThenBy(r => r.Reference)
                    .Select(r => new { r.Reference, r.Level })
                    .Distinct()
                    .ToList()
                    .ForEach(r =>
                    {
                        bool isPkg = r.Reference.EndsWith(":::pkg");
                        var reference = r.Reference.Replace(":::pkg", "");
                        var clasess = GetClasses(false, isPkg, r.Level);
                        contentBag.AppendLine($"<li{clasess}>[{r.Level}] {reference}</li>");
                    });
            }

            return contentBag.ToString();
        }

        private List<Relation> GetRelationsWithLevels(string element, IEnumerable<Relation> relations, HashSet<string> visitedElements = null, int level = 0) 
        {
            List<Relation> result = [];
            visitedElements ??= [];
            bool isNew = visitedElements.Add(element);

            if (isNew)
            {
                result = relations
                    .Where(r => r.Element == element)
                    .Select(r => new Relation { Element = element, Reference = r.Reference, Level = level + 1 })
                    .ToList();

                List<Relation> newRelations = [];


                foreach (Relation relation in result)
                {
                    newRelations.AddRange(GetRelationsWithLevels(relation.Reference, relations, visitedElements, level + 1));
                }
                result.AddRange(newRelations);
            }

            return result;
        }

        private static string GetClasses(bool isPinned, bool isPkg, int level = -1)
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
            if (level > 0)
            {
                clasess.Add($"level-{level}");
            }

            return clasess.Count > 0 ? $" class=\"{string.Join(" ", clasess)}\"" : "";
        }

        public string RenderToHtml(string input, UserSettings userSettings)
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
                            height: 98%;
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
