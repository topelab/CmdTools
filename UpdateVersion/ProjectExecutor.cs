namespace UpdateVersion
{
    using CmdTools.Contracts;
    using CmdTools.Shared;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    internal class ProjectExecutor(IFileExecutorFactory fileExecutorFactory,
                                   IVersionSplitter versionSplitter,
                                   IProjectUpdaterFactory projectUpdaterFactory,
                                   IVersionBumper versionBumper) : IProjectExecutor
    {
        private readonly IFileExecutorFactory fileExecutorFactory = fileExecutorFactory ?? throw new ArgumentNullException(nameof(fileExecutorFactory));
        private readonly IVersionSplitter versionSplitter = versionSplitter ?? throw new ArgumentNullException(nameof(versionSplitter));
        private readonly IProjectUpdaterFactory projectUpdaterFactory = projectUpdaterFactory ?? throw new ArgumentNullException(nameof(projectUpdaterFactory));
        private readonly IVersionBumper versionBumper = versionBumper ?? throw new ArgumentNullException(nameof(versionBumper));

        public void Run<T>(T args) where T : class
        {
            if (args is not Options options)
            {
                throw new ArgumentException("Invalid options type", nameof(args));
            }

            var directoryBuildPropertiesFileName = GetDirectoryBuildPropertiesFileName(options.BasePath ?? AppContext.BaseDirectory);

            if (File.Exists(directoryBuildPropertiesFileName))
            {
                TryUpdateDirectoryBuildProperties(directoryBuildPropertiesFileName, options);
            }
            else
            {
                TryUpdateProjects(options);
            }

        }

        private void TryUpdateDirectoryBuildProperties(string directoryBuildPropertiesFileName, Options options)
        {
            var projectUpdaterType = ProjectUpdaterType.DirectoryBuildProperties;
            string basePath = options.BasePath ?? AppContext.BaseDirectory;

            Dictionary<string, string> versionsMap = TryGetDirectoryBuildPropertyVersions(directoryBuildPropertiesFileName);
            if (versionBumper.TryBump(options.VersionsToBump, versionsMap) && options.Update is true)
            {
                IProjectUpdater projectUpdater = projectUpdaterFactory.Create(projectUpdaterType);
                projectUpdater.Update(directoryBuildPropertiesFileName, versionsMap);
                var referencesBag = new ReferencesBag();
                var fileExecutor = fileExecutorFactory.Create(basePath, Constants.FilePattern);
                fileExecutor.RunOnFiles(file => TryShowVersionUpdated(file, versionsMap, referencesBag));
                ShowVersions(versionsMap, referencesBag);
            }
        }

        private static void ShowVersions(Dictionary<string, string> versionsMap, ReferencesBag referencesBag)
        {
            if (referencesBag.Count != 0)
            {
                if (referencesBag.Count == 1)
                {
                    Console.WriteLine($"Version {referencesBag.Keys.First()}");
                }
                else
                {
                    Console.WriteLine($"Versions {string.Join(", ", referencesBag.Select(r => $"{r.Key} ({string.Join(", ", r.Value)})"))}");
                }
            }
        }

        private void TryShowVersionUpdated(string file, Dictionary<string, string> versionsMap, ReferencesBag referencesBag)
        {
            var fileContent = File.ReadAllText(file);
            foreach (var projectPattern in versionsMap.Keys)
            {
                var version = versionsMap[projectPattern];
                var projectName = $"$({projectPattern})";
                if (fileContent.Contains(projectName))
                {
                    referencesBag.AddReference(version, Path.GetFileNameWithoutExtension(file));
                    break;
                }
            }
        }

        private void TryUpdateProjects(Options options)
        {
            Dictionary<string, string> versionsMap = [];
            IEnumerable<string> versions = options.Versions;
            string basePath = options.BasePath ?? AppContext.BaseDirectory;

            versionsMap = versions.Any() ? TryGetVersions(versions) : TryGetVersions(basePath, options.VersionsFile, options.VersionsToBump);
            var referencesBag = new ReferencesBag();
            var fileExecutor = fileExecutorFactory.Create(basePath, Constants.FilePattern);

            if (options.Update is true)
            {
                fileExecutor.RunOnFiles(file => TryUpdateProject(file, versionsMap, referencesBag));
                ShowVersions(versionsMap, referencesBag);
            }
        }

        private void TryUpdateProject(string file, Dictionary<string, string> versionsMap, ReferencesBag referencesBag)
        {
            IProjectUpdater projectUpdater = projectUpdaterFactory.Create(ProjectUpdaterType.Projects);

            string projectName = Path.GetFileNameWithoutExtension(file);
            var version = GetMatchVersion(projectName, versionsMap);
            if (version != null)
            {
                projectUpdater.Update(file, version);
                referencesBag.AddReference(version, projectName);
            }
        }

        private Dictionary<string, string> TryGetDirectoryBuildPropertyVersions(string directoryBuildPropertiesFileName)
        {
            const string PropertyGroupNodeName = "PropertyGroup";
            Dictionary<string, string> versionsMap = [];

            XDocument document = XDocument.Load(directoryBuildPropertiesFileName);

            document.Descendants().Where(d => d.Name.LocalName == PropertyGroupNodeName)
                .Descendants()
                .Select(d => new { Name = d.Name.LocalName, Version = d.Value })
                .Where(x => x.Version.Count(c => c == '.') >= 2)
                .ToDictionary(x => x.Name, x => x.Version)
                .ToList()
                .ForEach(x => versionsMap.Add(x.Key, x.Value));

            return versionsMap;
        }

        public string Get<T>(T args) where T : class
        {
            throw new NotImplementedException();
        }

        private static string GetDirectoryBuildPropertiesFileName(string basePath)
        {
            return Path.Combine(basePath, "Directory.Build.props");
        }

        private Dictionary<string, string> TryGetVersions(IEnumerable<string> versions)
        {
            var versionsMap = new Dictionary<string, string>();
            UpdateVersionMap(versionsMap, versions);
            return versionsMap;
        }

        private Dictionary<string, string> TryGetVersions(string basePath, string versionsFileName, IEnumerable<string> versionsToBump)
        {
            var versionsMap = new Dictionary<string, string>();
            var file = Path.Combine(basePath, versionsFileName);

            if (File.Exists(file))
            {
                var versionContent = File.ReadAllText(file);
                var lines = versionContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                UpdateVersionMap(versionsMap, lines);
                versionBumper.Bump(file, versionsToBump, versionsMap);
            }
            else
            {
                versionsMap.Add(Constants.AnyProjectSelector, Constants.DefaultVersion);
            }

            return versionsMap;
        }

        private void UpdateVersionMap(Dictionary<string, string> versionsMap, IEnumerable<string> versions)
        {
            if (versions.Any())
            {
                foreach (var line in versions)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var (projectPattern, version) = versionSplitter.Split(line, Constants.AnyProjectSelector);
                        versionsMap.Add(projectPattern, version);
                    }
                }
            }
            else
            {
                versionsMap.Add(Constants.AnyProjectSelector, Constants.DefaultVersion);
            }
        }

        private static string GetMatchVersion(string file, Dictionary<string, string> versionsMap)
        {
            var globalVersion = versionsMap.TryGetValue(Constants.AnyProjectSelector, out var value) ? value : null;
            var version = globalVersion;
            foreach (var projectPattern in versionsMap.Keys.Where(k => k != Constants.AnyProjectSelector))
            {
                var partialName = projectPattern.Trim('*');
                bool matched = file.Equals(projectPattern, StringComparison.CurrentCultureIgnoreCase)
                    || (projectPattern.StartsWith(Constants.AnyProjectSelector) && file.EndsWith(partialName, StringComparison.CurrentCultureIgnoreCase))
                    || (projectPattern.EndsWith(Constants.AnyProjectSelector) && file.StartsWith(partialName, StringComparison.CurrentCultureIgnoreCase))
                    || (projectPattern.StartsWith(Constants.AnyProjectSelector) && projectPattern.EndsWith(Constants.AnyProjectSelector) && file.Contains(partialName, StringComparison.CurrentCultureIgnoreCase));

                if (matched)
                {
                    version = versionsMap[projectPattern];
                }
            }
            return version;
        }
    }
}
