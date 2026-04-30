namespace CreateRelationsDiagram
{
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using RelationsShared.Services;
    using System.Text;

    internal class SimpleTextRender : IOutputRender
    {
        public string Create(IEnumerable<Relation> relations, Options options)
        {
            var contentBag = new StringBuilder();
            var pinnedElement = options.PinnedElement;
            relations.Select(r => r.Element)
                .Union(relations.Select(r => r.Reference))
                .Distinct()
                .Where(e => e != pinnedElement)
                .OrderBy(e => e)
                .ToList()
                .ForEach(r =>
                {
                    bool isPkg = r.EndsWith(":::pkg");
                    r = r.Replace(":::pkg", " 📦 ");
                    contentBag.AppendLine($"- {r}");
                });

            return contentBag.ToString();
        }


        public string RenderToHtml(string input, UserSettings userSettings)
        {
            return "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><title>Relations Diagram</title></head><body><pre>" + input + "</pre></body></html>";
        }
    }
}