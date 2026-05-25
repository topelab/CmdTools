namespace UpdateVersion
{
    using CmdTools.Contracts;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class ProjectExecutor(IFileExecutorFactory fileExecutorFactory,
                                   IProjectUpdater projectUpdater,
                                   IVersionSplitter versionSplitter,
                                   IVersionBumper versionBumper) : IProjectExecutor
    {
        private readonly IFileExecutorFactory fileExecutorFactory = fileExecutorFactory ?? throw new ArgumentNullException(nameof(fileExecutorFactory));
        private readonly IProjectUpdater projectUpdater = projectUpdater ?? throw new ArgumentNullException(nameof(projectUpdater));
        private readonly IVersionSplitter versionSplitter = versionSplitter ?? throw new ArgumentNullException(nameof(versionSplitter));
        private readonly IVersionBumper versionBumper = versionBumper ?? throw new ArgumentNullException(nameof(versionBumper));

        public void Run<T>(T args) where T : class
        {
            if (args is not Options options)
            {
                throw new ArgumentException("Invalid options type", nameof(args));
            }

            string basePath = options.BasePath ?? AppContext.BaseDirectory;
            IEnumerable<string> versions = options.Versions;
            if (ExistDirectoryBuildProps(basePath))
            {
                Console.WriteLine("Directory.Build.props found. Please, update versions there.");
                return;
            }

            var versionsMap = versions.Any() ? TryGetVersions(versions) : TryGetVersions(basePath, options.VersionsFile, options.VersionsToBump);
            var fileExecutor = fileExecutorFactory.Create(basePath, Constants.FilePattern);
            if (options?.Update ?? false)
            {
                fileExecutor.RunOnFiles(file => TryUpdate(file, versionsMap));
            }
        }

        public string Get<T>(T args) where T : class
        {
            throw new NotImplementedException();
        }

        private bool ExistDirectoryBuildProps(string basePath)
        {
            var file = Path.Combine(basePath, "Directory.Build.props");
            return File.Exists(file);
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

        private void TryUpdate(string file, Dictionary<string, string> versionsMap)
        {
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
