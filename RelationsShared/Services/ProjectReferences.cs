namespace RelationsShared.Services
{
    using CmdTools.Contracts;
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    internal class ProjectReferences(IFileExecutorFactory fileExecutorFactory) : IProjectReferences
    {
        private readonly IFileExecutorFactory fileExecutorFactory = fileExecutorFactory;

        private bool withPackages;
        private Regex excludeProjects;
        private readonly Dictionary<string, string> packageVersions = [];
        private readonly List<ProjectRelation> projectRelations = [];

        public void Initialize(bool withPackages, Regex excludeProjects, string path)
        {
            this.withPackages = withPackages;
            this.excludeProjects = excludeProjects;
            InitializePackageVersions(path);
            projectRelations.Clear();
        }

        public void Initialize(ProjectRelationsContext context)
        {
            excludeProjects = context.ExcludeElements;
            InitializePackages(context);
            InitializeProjectsRelations(context);
        }

        private void InitializePackages(ProjectRelationsContext context)
        {
            var options = context.Options as ProjectOptions;
            withPackages = options.WithPackages;
            packageVersions.Clear();
            if (withPackages)
            {
                context.PackageVersions.Keys.ToList().ForEach(k => packageVersions[k] = context.PackageVersions[k]);
            }
        }

        private void InitializeProjectsRelations(ProjectRelationsContext context)
        {
            projectRelations.Clear();
            context.ElementsRelations.Keys
                .ToList()
                .ForEach(r =>
                {
                    var relations = context.ElementsRelations[r]
                        .Where(pr => withPackages || pr.RelationType != ProjectRelationType.PackageReference)
                        .Select(pr => new ProjectRelation(r, pr));

                    projectRelations.AddRange(relations);
                });
        }

        private void InitializePackageVersions(string path)
        {
            packageVersions.Clear();
            var fileExecutor = fileExecutorFactory.Create(path, Constants.PackagesFilePattern);
            fileExecutor.RunOnFiles(file =>
            {
                XDocument document = XDocument.Load(file);
                var packageReferences = document.Descendants()
                    .Where(d => d.Name.LocalName == "PackageVersion")
                    .Where(d => d.Attribute("Include") != null)
                    .Select(d => new { Name = d.Attribute("Include").Value, Version = ExtractVersion(d) })
                    .Where(e => e.Name != null);

                foreach (var package in packageReferences)
                {
                    packageVersions[package.Name] = package.Version;
                }
            });

        }

        public IEnumerable<ElementRelation> GetProjectRelations(string projectPath, HashSet<string> currentProjects = null, string basePath = null)
        {
            List<ElementRelation> currentProjectRelations = [];
            currentProjects ??= [];
            var localBasePath = GetFullPath(basePath, projectPath);
            var projectName = Path.GetFileNameWithoutExtension(projectPath);
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
                    .Select(path => new ProjectElement(Path.GetFileNameWithoutExtension(path), path))
                    .ToList();

                var packageReferences = withPackages
                    ? document.Descendants()
                        .Where(node => node.Name.LocalName == "PackageReference")
                        .Where(node => node.Attribute("Include") != null)
                        .Select(node => new { Name = node.Attribute("Include").Value, Version = GetPackageVersion(node) })
                        .Where(e => e.Name != null)
                        .Select(e => new PackageElement(e.Name, e.Version, "📦"))
                        .ToList()
                    : [];

                projectReferences.ForEach(projectRelation => currentProjectRelations.AddRange(GetProjectRelations(projectRelation.Path, currentProjects, Path.GetDirectoryName(localBasePath))));

                currentProjectRelations.AddRange(projectReferences);
                currentProjectRelations.AddRange(packageReferences);
            }

            return currentProjectRelations;
        }

        public IEnumerable<string> GetProjects(string projectPath, HashSet<string> currentProjects = null, string basePath = null)
        {
            List<string> result = [];
            currentProjects ??= [];
            var projectName = Path.GetFileNameWithoutExtension(projectPath);

            var projectReferences = projectRelations
                .Where(r => r.Parent == projectName)
                .Select(r => r.Child)
                .OfType<ProjectElement>()
                .Where(r => !currentProjects.Contains(r.Path))
                .Select(r => r.Path)
                .ToList();

            projectReferences.ForEach(reference => currentProjects.Add(reference));
            projectReferences.ForEach(reference => result.AddRange(GetProjects(reference, currentProjects, Path.GetDirectoryName(GetFullPath(basePath, projectPath)))));

            result.AddRange(projectReferences);

            return result.Distinct();
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


        private IEnumerable<string> Get(string projectPath)
        {
            return projectRelations
                .Where(r => r.Parent == projectPath)
                .Select(r => $"{r.Child}")
                .Distinct();
        }

        private string GetPackageVersion(XElement node)
        {
            string packageName = node.Attribute("Include").Value;
            string originalVersion = ExtractVersion(node);
            string version;
            if (!string.IsNullOrEmpty(originalVersion))
            {
                version = originalVersion;
            }
            else
            {
                if (!packageVersions.TryGetValue(packageName, out version))
                {
                    version = string.Empty;
                }
            }
            return version;
        }

        private static string ExtractVersion(XElement node)
        {
            var version = node.Attribute("Version")?.Value ?? node.Attribute("version")?.Value;
            if (string.IsNullOrEmpty(version))
            {
                version = node.Descendants().FirstOrDefault(e => e.Name.LocalName == "Version")?.Value;
            }
            return version;
        }

        private static string GetFullPath(string basePath, string projectPath)
        {
            return Path.GetFullPath(basePath is null ? projectPath : Path.Combine(basePath, projectPath));
        }
    }
}
