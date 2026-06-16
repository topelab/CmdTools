namespace UpdateVersion
{
    using CmdTools.Contracts;
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

            string basePath = options.BasePath ?? AppContext.BaseDirectory;
            var directoryBuildPropertiesFileName = GetDirectoryBuildPropertiesFileName(basePath);
            Dictionary<string, string> versionsMap = [];

            IEnumerable<string> versions = options.Versions;
            ProjectUpdaterType projectUpdaterType = ProjectUpdaterType.Projects;

            if (File.Exists(directoryBuildPropertiesFileName))
            {
                versionsMap = TryGetDirectoryBuildPropertyVersions(directoryBuildPropertiesFileName);
                projectUpdaterType = ProjectUpdaterType.DirectoryBuildProperties;
                if (versionBumper.TryBump(options.VersionsToBump, versionsMap))
                {
                    IProjectUpdater projectUpdater = projectUpdaterFactory.Create(projectUpdaterType);
                    projectUpdater.Update(directoryBuildPropertiesFileName, versionsMap);
                }
            }
            else
            {
                versionsMap = versions.Any() ? TryGetVersions(versions) : TryGetVersions(basePath, options.VersionsFile, options.VersionsToBump);
                var fileExecutor = fileExecutorFactory.Create(basePath, Constants.FilePattern);

                if (options?.Update ?? false)
                {
                    fileExecutor.RunOnFiles(file => TryUpdate(file, versionsMap, projectUpdaterType));
                }
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

        private void TryUpdate(string file, Dictionary<string, string> versionsMap, ProjectUpdaterType projectUpdaterType)
        {
            IProjectUpdater projectUpdater = projectUpdaterFactory.Create(projectUpdaterType);

            string projectName = Path.GetFileNameWithoutExtension(file);
            var version = GetMatchVersion(projectName, versionsMap);
            if (version != null)
            {
                projectUpdater.Update(file, version);
                Console.WriteLine($"{file} to {GetMatchVersion(projectName, versionsMap)}");
            }
        }


        private void TryUpdateProperties(string file, Dictionary<string, string> versionsMap, ProjectUpdaterType projectUpdaterType)
        {
            IProjectUpdater projectUpdater = projectUpdaterFactory.Create(projectUpdaterType);

            versionsMap.Where(r => r.Value.Count(c => c == '.') >= 3).ToList().ForEach(r =>
            {
                projectUpdater.Update(r.Key, r.Value);
            });

            string projectName = Path.GetFileNameWithoutExtension(file);
            var version = GetMatchVersion(projectName, versionsMap);
            if (version != null)
            {
                projectUpdater.Update(file, version);
                Console.WriteLine($"{file} to {GetMatchVersion(projectName, versionsMap)}");
            }
        }

        private static string GetMatchVersion(string file, Dictionary<string, string> versionsMap)
        {
            var globalVersion = versionsMap.TryGetValue(Constants.AnyProjectSelector, out var value) ? value : null;
            var version = globalVersion;
            foreach(var projectPattern in versionsMap.Keys.Where(k => k != Constants.AnyProjectSelector))
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
