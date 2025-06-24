using System.Xml.Linq;

namespace CreateRelationsDiagram
{
    internal class ProjectReferences : IProjectReferences
    {
        public IEnumerable<string> Get(string projectPath)
        {
            XDocument document = XDocument.Load(projectPath);
            var projectReferences = document.Descendants()
                .Where(d => d.Name.LocalName == "ProjectReference")
                .Select(d => d.Attribute("Include")?.Value)
                .Where(v => !string.IsNullOrEmpty(v));

            return projectReferences.Select(r => Path.GetFileNameWithoutExtension(r)).Where(v => !string.IsNullOrEmpty(v));
        }

        private static string GetProjectName(string v)
        {
            var parts = v.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries);
            return parts.LastOrDefault()?.Replace(".csproj", string.Empty) ?? string.Empty;
        }
    }
}
