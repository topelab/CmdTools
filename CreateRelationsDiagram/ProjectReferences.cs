namespace CreateRelationsDiagram
{
    using System.Xml.Linq;

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
    }
}
