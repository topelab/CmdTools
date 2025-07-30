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
                .Where(d => d.Attribute("Include") != null)
                .Select(d => d.Attribute("Include").Value);

            var packageReferences = document.Descendants()
                .Where(d => d.Name.LocalName == "PackageReference")
                .Where(d => d.Attribute("Include") != null)
                .Select(d => new { Name = d.Attribute("Include").Value, Version = d.Attribute("Version")?.Value })
                .Select(d => $"{d.Name}{(string.IsNullOrEmpty(d.Version) ? string.Empty : "-")}{d.Version}");

            return projectReferences
                .Select(r => Path.GetFileNameWithoutExtension(r))
                .Where(v => !string.IsNullOrEmpty(v))
                .Union(packageReferences);
        }
    }
}
