namespace RelationsShared.Services
{
    using CmdTools.Contracts;
    using CmdTools.Contracts.DTO;
    using RelationsShared.DTO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    internal class ProjectRelationsInitializer : IElementRelationsInitializer<ProjectRelationsContext>
    {
        private readonly IFileExecutor fileExecutor;

        public ProjectRelationsInitializer(IFileExecutor fileExecutor)
        {
            this.fileExecutor = fileExecutor;
        }

        private ProjectRelationsContext context;

        public void Initialize(ProjectRelationsContext context)
        {
            this.context = context;
            var options = context.Options;
            options.RootPath ??= Environment.ProcessPath;
            options.InitialPath ??= options.RootPath;

            var path = options.RootPath;
            context.ExcludeProjects = string.IsNullOrEmpty(options.Exclude) ? null : new Regex(options.Exclude, RegexOptions.IgnoreCase);
            context.PackageVersions = GetPackageVersions(options.InitialPath);

            options.OutputFile = options.OutputFile?.Replace(".csproj", string.Empty, StringComparison.CurrentCultureIgnoreCase);
            options.PinnedElement = options.PinnedElement?.Replace(".csproj", string.Empty, StringComparison.CurrentCultureIgnoreCase);

            var projectFiles = GetProjectFiles();
            options.ProjectPaths.Clear();
            options.ProjectPaths.AddRange(projectFiles);

            HashSet<string> currentProjects = [];
            foreach (var project in options.ProjectPaths)
            {
                if (currentProjects.Contains(project))
                {
                    continue;
                }
                context.ProjectRelations.AddReferences(project, GetProjectRelations(project, currentProjects));
                currentProjects.Add(project);
            }
            context.Projects = [.. currentProjects.Select(p => Path.GetFileNameWithoutExtension(p)).OrderBy(p => p)];
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
                    .Where(path => context.ExcludeProjects == null || !context.ExcludeProjects.IsMatch(path))
                    .Where(path => File.Exists(path))
                    .Select(path => new ProjectElement(Path.GetFileNameWithoutExtension(path), path))
                    .ToList();

                var packageReferences = context.Options.WithPackages
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


        private HashSet<string> GetProjectFiles()
        {
            fileExecutor.Initialize(context.Options.RootPath, Constants.FilePattern, context.ExcludeProjects);

            HashSet<string> projectFiles = [];
            fileExecutor.RunOnFiles(file => projectFiles.Add(file));
            return projectFiles;
        }

        private Dictionary<string, string> GetPackageVersions(string path)
        {
            Dictionary<string, string> packageVersions = [];
            fileExecutor.Initialize(path, Constants.PackagesFilePattern);
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

            return packageVersions;
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
                if (!context.PackageVersions.TryGetValue(packageName, out version))
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
