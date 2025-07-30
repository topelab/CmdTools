namespace CreateRelationsDiagram
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    internal class ProjectReferences : IProjectReferences
    {
        private bool withPackages;

        public void Initialize(bool withPackages)
        {
            this.withPackages = withPackages;
        }

        public IEnumerable<string> Get(string projectPath)
        {
            XDocument document = XDocument.Load(projectPath);
            var projectReferences = document.Descendants()
                .Where(d => d.Name.LocalName == "ProjectReference")
                .Where(d => d.Attribute("Include") != null)
                .Select(d => d.Attribute("Include").Value);

            var packageReferences = withPackages ? document.Descendants()
                .Where(d => d.Name.LocalName == "PackageReference")
                .Where(d => d.Attribute("Include") != null)
                .Select(d => new { Name = d.Attribute("Include").Value, Version = d.Attribute("Version")?.Value })
                .Select(d => $"{d.Name}{(string.IsNullOrEmpty(d.Version) ? string.Empty : "-")}{d.Version}:::pkg") : [];

            return projectReferences
                .Select(r => Path.GetFileNameWithoutExtension(r))
                .Where(v => !string.IsNullOrEmpty(v))
                .Union(packageReferences);
        }

        public IEnumerable<string> GetProjects(string projectPath, HashSet<string> currentProjects = null, string basePath = null)
        {
            List<string> result = [];
            currentProjects ??= [];
            var localBasePath = GetFullPath(basePath, projectPath);
            XDocument document = XDocument.Load(localBasePath);
            var projectReferences = document.Descendants()
                .Where(d => d.Name.LocalName == "ProjectReference")
                .Where(d => d.Attribute("Include") != null)
                .Select(d => GetFullPath(Path.GetDirectoryName(localBasePath), d.Attribute("Include").Value))
                .Where(p => !currentProjects.Contains(p))
                .ToList();

            projectReferences.ForEach(reference => currentProjects.Add(reference));
            projectReferences.ForEach(reference => result.AddRange(GetProjects(reference, currentProjects, Path.GetDirectoryName(GetFullPath(basePath, projectPath)))));

            result.AddRange(projectReferences);

            return result.Distinct();
        }

        private static string GetFullPath(string basePath, string projectPath)
        {
            return Path.GetFullPath(basePath is null ? projectPath : Path.Combine(basePath, projectPath));
        }
    }
}
