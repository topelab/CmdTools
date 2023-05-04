using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;

namespace UpdateVersion
{
    internal class ProjectFinder : IProjectFinder
    {
        private const string AnyProjectSelector = "*";
        private const string BumpAllProjects = "ALL";
        private const string DefaultVersion = "1.0.0";
        private const string FilePattern = "*.csproj";
        private const char ProjectVersionSeparator = ':';
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

            var versionsMap = versions.Any() ? TryGetVersions(versions) : TryGetVersions(basePath, options.VersionsFile, options.VersionsToBump);
            fileExecutor.Initialize(basePath, FilePattern);
            fileExecutor.RunOnFiles(file => TryUpdate(file, versionsMap));
        }

        private static Dictionary<string, string> TryGetVersions(IEnumerable<string> versions)
        {
            var versionsMap = new Dictionary<string, string>();
            UpdateVersionMap(versionsMap, versions);
            return versionsMap;
        }

        private static Dictionary<string, string> TryGetVersions(string basePath, string versionsFileName, IEnumerable<string> versionsToBump)
        {
            var versionsMap = new Dictionary<string, string>();
            var file = Path.Combine(basePath, versionsFileName);

            if (File.Exists(file))
            {
                var versionContent = File.ReadAllText(file);
                var lines = versionContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                UpdateVersionMap(versionsMap, lines);
                BumpToVersion(file, versionsToBump, versionsMap);
            }
            else
            {
                versionsMap.Add(AnyProjectSelector, DefaultVersion);
            }

            return versionsMap;
        }

        private static void BumpToVersion(string file, IEnumerable<string> versionsToBump, Dictionary<string, string> versionsMap)
        {
            if (versionsToBump.Any())
            {
                bool modified = false;
                string pattern;
                string versionLevel;

                var globalBump = versionsToBump.FirstOrDefault(v => !v.Contains(ProjectVersionSeparator) || v.StartsWith(BumpAllProjects));
                if (globalBump != null)
                {
                    (pattern, versionLevel) = GetNameAndValue(globalBump, BumpAllProjects);
                    foreach (var key in versionsMap.Keys)
                    {
                        modified = BumpVersionMapWithPatern(versionsMap, key, versionLevel) || modified;
                    }
                }
                else
                {
                    foreach (var item in versionsToBump)
                    {
                        (pattern, versionLevel) = GetNameAndValue(item, BumpAllProjects);
                        modified = BumpVersionMapWithPatern(versionsMap, pattern, versionLevel) || modified;
                    }
                }

                if (modified)
                {
                    File.WriteAllLines(file, versionsMap.Select(d => $"{d.Key}: {d.Value}").ToArray());
                }
            }
        }

        private static bool BumpVersionMapWithPatern(Dictionary<string, string> versionsMap, string pattern, string versionLevel)
        {
            bool modified = false;

            if (versionsMap.ContainsKey(pattern))
            {
                int level = int.Parse(versionLevel);
                string version = GetBumpedVersion(versionsMap[pattern], level - 1);
                versionsMap[pattern] = version;
                modified = true;
            }

            return modified;
        }

        private static string GetBumpedVersion(string version, int level)
        {
            var versionsPart = version.Split('.');
            if (level < versionsPart.Length)
            {
                versionsPart[level] = (int.Parse(versionsPart[level]) + 1).ToString();
                version = string.Join('.', versionsPart);
            }
            return version;
        }

        private static void UpdateVersionMap(Dictionary<string, string> versionsMap, IEnumerable<string> versions)
        {
            if (versions.Any())
            {
                foreach (var line in versions)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var (projectPattern, version) = GetNameAndValue(line, AnyProjectSelector);
                        versionsMap.Add(projectPattern, version);
                    }
                }
            }
            else
            {
                versionsMap.Add(AnyProjectSelector, DefaultVersion);
            }
        }

        private static (string name, string value) GetNameAndValue(string item, string defaultName)
        {
            string name;
            string value;

            if (item.Contains(ProjectVersionSeparator))
            {
                var parts = item.Split(ProjectVersionSeparator);
                name = parts[0].Trim();
                value = parts[1].Trim();
            }
            else
            {
                name = defaultName;
                value = item.Trim();
            }

            return (name, value);
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
            var globalVersion = versionsMap.TryGetValue(AnyProjectSelector, out var value) ? value : null;
            var version = globalVersion;
            foreach(var projectPattern in versionsMap.Keys.Where(k => k != AnyProjectSelector))
            {
                var partialName = projectPattern.Trim('*');
                bool matched = file.Equals(projectPattern, StringComparison.CurrentCultureIgnoreCase)
                    || (projectPattern.StartsWith(AnyProjectSelector) && file.EndsWith(partialName, StringComparison.CurrentCultureIgnoreCase))
                    || (projectPattern.EndsWith(AnyProjectSelector) && file.StartsWith(partialName, StringComparison.CurrentCultureIgnoreCase))
                    || (projectPattern.StartsWith(AnyProjectSelector) && projectPattern.EndsWith(AnyProjectSelector) && file.Contains(partialName, StringComparison.CurrentCultureIgnoreCase));

                if (matched) 
                { 
                    version = versionsMap[projectPattern];
                }
            }
            return version;
        }
    }
}
