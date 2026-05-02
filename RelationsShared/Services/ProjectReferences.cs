namespace RelationsShared.Services
{
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    internal class ProjectReferences : IProjectReferences
    {
        private bool withPackages;
        private Regex excludeProjects;
        private Dictionary<string, string> packageVersions = [];

        public void Initialize(bool withPackages, Regex excludeProjects)
        {
            this.withPackages = withPackages;
            this.excludeProjects = excludeProjects;
            InitializePackageVersions();
        }

        private void InitializePackageVersions()
        {
            packageVersions.Clear();
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
                .Where(r => excludeProjects == null || !excludeProjects.IsMatch(r))
                .Select(r => Path.GetFileNameWithoutExtension(r))
                .Where(v => !string.IsNullOrEmpty(v))
                .Union(packageReferences);
        }

        public IEnumerable<ProjectRelation> GetRelations(string projectPath, HashSet<string> currentProjects = null, string basePath = null)
        {
            List<ProjectRelation> projectRelations = [];
            currentProjects ??= [];
            var localBasePath = GetFullPath(basePath, projectPath);
            if (File.Exists(localBasePath))
            {
                XDocument document = XDocument.Load(localBasePath);
                var projectReferences = document.Descendants()
                    .Where(node => node.Name.LocalName == "ProjectReference")
                    .Where(node => node.Attribute("Include") != null)
                    .Select(node => GetFullPath(Path.GetDirectoryName(localBasePath), node.Attribute("Include").Value))
                    .Where(path => !currentProjects.Contains(path))
                    .Where(path => excludeProjects == null || !excludeProjects.IsMatch(path))
                    .Where(path => File.Exists(path))
                    .Select(path => new ProjectRelation(ProjectRelationType.ProjectReference, Path.GetFileNameWithoutExtension(path), path))
                    .ToList();

                var packageReferences = withPackages ? document.Descendants()
                    .Where(node => node.Name.LocalName == "PackageReference")
                    .Where(node => node.Attribute("Include") != null)
                    .Select(node => new { Name = node.Attribute("Include").Value, Version = ExtractVersion(node) })
                    .Where(e => e.Name != null)
                    .Select(e => new ProjectRelation(ProjectRelationType.PackageReference, e.Name, $"{e.Version} [📦]"))
                    .ToList() : [];


                projectReferences.ForEach(projectRelation => currentProjects.Add(projectRelation.AdditionalInfo));
                projectReferences.ForEach(projectRelation => projectRelations.AddRange(GetRelations(projectRelation.AdditionalInfo, currentProjects, Path.GetDirectoryName(localBasePath))));

                projectRelations.AddRange(projectReferences
                    .Union(packageReferences));
            }

            return projectRelations.Distinct();
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
                    .Where(r => excludeProjects == null || !excludeProjects.IsMatch(r))
                    .Where(p => File.Exists(p))
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
