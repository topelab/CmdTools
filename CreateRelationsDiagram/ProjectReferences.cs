namespace CreateRelationsDiagram
{
    using CmdTools.Shared;
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
            if (!File.Exists(projectPath))
            {
                return [];
            }

            XDocument document = XDocument.Load(projectPath);
            var projectReferences = document.Descendants()
                .Where(d => d.Name.LocalName == "ProjectReference")
                .Where(d => d.Attribute("Include") != null)
                .Select(d => d.Attribute("Include").Value);

            var packageReferences = withPackages ? document.Descendants()
                .Where(d => d.Name.LocalName == "PackageReference")
                .Where(d => d.Attribute("Include") != null)
                .Select(d => new { Name = d.Attribute("Include").Value, Version = ExtractVersion(d) })
                .Select(d => $"{d.Name}{(string.IsNullOrEmpty(d.Version) ? string.Empty : "-")}{d.Version}:::pkg") : [];

            return projectReferences
                .Select(r => Path.GetFileNameWithoutExtension(r))
                .Where(v => !string.IsNullOrEmpty(v))
                .Union(packageReferences);
        }

        public ReferencesBag GetReferences(HashSet<string> projectFiles)
        {
            ReferencesBag references = [];
            foreach (var file in projectFiles)
            {
                references[Path.GetFileNameWithoutExtension(file)] = [.. Get(file)];
            }

            return references;
        }

        public ReferencesBag GetInverseReferences(HashSet<string> projectFiles)
        {
            ReferencesBag references = [];
            foreach (var file in projectFiles)
            {
                Get(file).ToList().ForEach(r => references.AddReference(r, Path.GetFileNameWithoutExtension(file)));
            }

            return references;
        }


        private static string ExtractVersion(XElement d)
        {
            var version = d.Attribute("Version")?.Value ?? d.Attribute("version")?.Value;
            if (string.IsNullOrEmpty(version))
            {
                version = d.Descendants().FirstOrDefault(e => e.Name.LocalName == "Version")?.Value;
            }
            return version;
        }

        public IEnumerable<string> GetProjects(string projectPath, HashSet<string> currentProjects = null, string basePath = null)
        {
            List<string> result = [];
            currentProjects ??= [];
            var localBasePath = GetFullPath(basePath, projectPath);
            if (File.Exists(localBasePath))
            {
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
            }

            return result.Distinct();
        }

        private static string GetFullPath(string basePath, string projectPath)
        {
            return Path.GetFullPath(basePath is null ? projectPath : Path.Combine(basePath, projectPath));
        }
    }
}
