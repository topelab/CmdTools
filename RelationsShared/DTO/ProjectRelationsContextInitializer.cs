namespace RelationsShared.DTO
{
    using CmdTools.Contracts;
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using RelationsShared.Services;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    internal class ProjectRelationsContextInitializer(IFileExecutorFactory fileExecutorFactory) : IProjectRelationsContextInitializer
    {
        private readonly IFileExecutorFactory fileExecutorFactory = fileExecutorFactory;

        public void Initialize(RelationsContext relationsContext)
        {
            ProjectRelationsContext context = relationsContext as ProjectRelationsContext;
            var options = context.Options as ProjectOptions;

            context.ElementsRelations.Clear();
            context.PackageVersions.Clear();
            context.Elements.Clear();
            context.ExcludeElements = string.IsNullOrEmpty(options.Exclude) ? null : new Regex(options.Exclude, RegexOptions.IgnoreCase);

            options.RootPath ??= Environment.ProcessPath;
            options.InitialPath ??= options.RootPath;
            options.OutputFile = options.OutputFile?.Replace(".csproj", string.Empty, StringComparison.CurrentCultureIgnoreCase);
            options.PinnedElement = options.PinnedElement?.Replace(".csproj", string.Empty, StringComparison.CurrentCultureIgnoreCase);

            InitializeProjects(context);
        }

        private void InitializeProjects(ProjectRelationsContext context)
        {
            var options = context.Options as ProjectOptions;

            var projectFiles = GetProjectFiles(context);

            HashSet<string> currentProjects = [];
            foreach (var project in projectFiles)
            {
                if (currentProjects.Contains(project))
                {
                    continue;
                }
                currentProjects.Add(project);
                SetRelations(context, project, currentProjects);
            }
            context.Elements = [.. currentProjects.Distinct()];
            options.ProjectPaths.Clear();
            options.ProjectPaths.AddRange(context.Elements);
        }

        private HashSet<string> GetProjectFiles(ProjectRelationsContext context)
        {
            var options = context.Options as ProjectOptions;
            var fileExecutor = fileExecutorFactory.Create(options.RootPath, Constants.FilePattern, context.ExcludeElements);

            HashSet<string> projectFiles = [];
            fileExecutor.RunOnFiles(file => projectFiles.Add(file));
            return projectFiles;
        }

        private void SetRelations(ProjectRelationsContext context, string projectPath, HashSet<string> currentProjects = null, HashSet<string> currentPackages = null, string basePath = null)
        {
            currentPackages ??= [];
            ElementsRelations elementRelations = context.ElementsRelations;
            var references = GetProjectRelations(context, projectPath, currentPackages, currentProjects, basePath);
            elementRelations.AddReferences(projectPath, references);
            foreach (var reference in references)
            {
                if (reference is ProjectElement projectReference && !currentProjects.Contains(projectReference.Path))
                {
                    currentProjects.Add(projectReference.Path);
                    SetRelations(context, projectReference.Path, currentProjects, currentPackages, Path.GetDirectoryName(GetFullPath(basePath, projectPath)));
                }
            }
        }

        private List<ElementRelation> GetProjectRelations(ProjectRelationsContext context, string projectPath, HashSet<string> currentPackages, HashSet<string> currentProjects = null, string basePath = null)
        {
            List<ElementRelation> currentProjectRelations = [];
            currentProjects ??= [];
            var localBasePath = GetFullPath(basePath, projectPath);

            UpdatePackageVersions(context, localBasePath, currentPackages);

            var projectName = Path.GetFileNameWithoutExtension(projectPath);
            if (File.Exists(localBasePath))
            {
                XDocument document = XDocument.Load(localBasePath);
                var projectReferences = document.Descendants()
                    .Where(node => node.Name.LocalName == "ProjectReference")
                    .Where(node => node.Attribute("Include") != null)
                    .Select(node => GetFullPath(Path.GetDirectoryName(localBasePath), node.Attribute("Include").Value))
                    .Where(path => context.ExcludeElements == null || !context.ExcludeElements.IsMatch(path))
                    .Where(path => File.Exists(path))
                    .Select(path => new ProjectElement(Path.GetFileNameWithoutExtension(path), path))
                    .ToList();

                var packageReferences = document.Descendants()
                        .Where(node => node.Name.LocalName == "PackageReference")
                        .Where(node => node.Attribute("Include") != null)
                        .Select(node => new { Name = node.Attribute("Include").Value, Version = GetPackageVersion(context, node) })
                        .Where(e => e.Name != null)
                        .Select(e => new PackageElement(e.Name, e.Version, "📦"))
                        .ToList();

                currentProjectRelations.AddRange(projectReferences);
                currentProjectRelations.AddRange(packageReferences);
            }

            return currentProjectRelations;
        }

        private void UpdatePackageVersions(ProjectRelationsContext context, string localBasePath, HashSet<string> currentPackages)
        {
            var localPackageVersions = GetPackageVersions(Path.GetDirectoryName(Path.GetDirectoryName(localBasePath)), currentPackages);
            foreach (var packageName in localPackageVersions.Keys)
            {
                if (context.PackageVersions.TryGetValue(packageName, out var packageVersion))
                {
                    if (!context.PackageVersions[packageName].Contains(packageVersion))
                    {
                        context.PackageVersions[packageName] = $"{packageVersion},{localPackageVersions[packageName]}";
                    }
                }
                else
                {
                    context.PackageVersions.Add(packageName, localPackageVersions[packageName]);
                }
            }
        }

        private Dictionary<string, string> GetPackageVersions(string path, HashSet<string> currentPackages)
        {
            Dictionary<string, string> packageVersions = [];

            if (!currentPackages.Contains(path))
            {
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
                currentPackages.Add(path);
            }

            return packageVersions;
        }

        private string GetPackageVersion(ProjectRelationsContext context, XElement node)
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
