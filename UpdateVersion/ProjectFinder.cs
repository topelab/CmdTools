using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UpdateVersion
{
    internal class ProjectFinder : IProjectFinder
    {
        private readonly IFileExecutor fileExecutor;
        private readonly IProjectUpdater projectUpdater;

        public ProjectFinder(IFileExecutor fileExecutor, IProjectUpdater projectUpdater)
        {
            this.fileExecutor = fileExecutor ?? throw new ArgumentNullException(nameof(fileExecutor));
            this.projectUpdater = projectUpdater ?? throw new ArgumentNullException(nameof(projectUpdater));
        }

        public void Run(Options options)
        {
            string basePath = options.BasePath ?? AppContext.BaseDirectory;
            IEnumerable<string> versions = options.Versions;

            var versionsMap = versions.Any() ? TryGetVersions(versions) : TryGetVersions(basePath, options.VersionsFile);
            fileExecutor.Initialize(basePath, "*.csproj");
            fileExecutor.RunOnFiles(file => TryUpdate(file, versionsMap));
        }

        private static Dictionary<string, string> TryGetVersions(IEnumerable<string> versions)
        {
            var versionsMap = new Dictionary<string, string>();
            UpdateVersionMap(versionsMap, versions);
            return versionsMap;
        }

        private static Dictionary<string, string> TryGetVersions(string basePath, string versionsFileName)
        {
            var versionsMap = new Dictionary<string, string>();
            var file = Path.Combine(basePath, versionsFileName);

            if (File.Exists(file))
            {
                var versionContent = File.ReadAllText(file);
                var lines = versionContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                UpdateVersionMap(versionsMap, lines);
            }
            else
            {
                versionsMap.Add("*", "1.0.0");
            }

            return versionsMap;
        }

        private static void UpdateVersionMap(Dictionary<string, string> versionsMap, IEnumerable<string> versions)
        {
            if (versions.Any())
            {
                foreach (var line in versions)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line.Contains(':'))
                        {
                            var projectVersion = line.Split(':');
                            var projectPattern = projectVersion[0].Trim();
                            var version = projectVersion[1].Trim();
                            versionsMap.Add(projectPattern, version);
                        }
                        else
                        {
                            versionsMap.Add("*", line.Trim());
                        }
                    }
                }
            }
            else
            {
                versionsMap.Add("*", "1.0.0");
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
            var globalVersion = versionsMap.TryGetValue("*", out var value) ? value : null;
            var version = globalVersion;
            foreach(var projectPattern in versionsMap.Keys.Where(k => k != "*"))
            {
                var partialName = projectPattern.Trim('*');
                bool matched = file.Equals(projectPattern, StringComparison.CurrentCultureIgnoreCase)
                    || (projectPattern.StartsWith("*") && file.EndsWith(partialName, StringComparison.CurrentCultureIgnoreCase))
                    || (projectPattern.EndsWith("*") && file.StartsWith(partialName, StringComparison.CurrentCultureIgnoreCase))
                    || (projectPattern.StartsWith("*") && projectPattern.EndsWith("*") && file.Contains(partialName, StringComparison.CurrentCultureIgnoreCase));

                if (matched) 
                { 
                    version = versionsMap[projectPattern];
                }
            }
            return version;
        }
    }
}
