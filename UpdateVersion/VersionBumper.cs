using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UpdateVersion
{
    internal class VersionBumper : IVersionBumper
    {
        private readonly IVersionSplitter versionSplitter;

        public VersionBumper(IVersionSplitter versionSplitter)
        {
            this.versionSplitter = versionSplitter ?? throw new ArgumentNullException(nameof(versionSplitter));
        }

        public void Bump(string file, IEnumerable<string> versionsToBump, Dictionary<string, string> versionsMap)
        {
            if (versionsToBump.Any())
            {
                bool modified = false;
                string pattern;
                string versionLevel;
                string globalVersionPrefix = $"{Constants.BumpAllProjects}{Constants.ProjectVersionSeparator}";

                var globalBump = versionsToBump.FirstOrDefault(v => v.StartsWith(globalVersionPrefix));
                if (globalBump != null)
                {
                    (pattern, versionLevel) = versionSplitter.Split(globalBump, Constants.BumpAllProjects);
                    foreach (var key in versionsMap.Keys)
                    {
                        modified = BumpVersionMapWithPatern(versionsMap, key, versionLevel) || modified;
                    }
                }
                else
                {
                    foreach (var item in versionsToBump)
                    {
                        (pattern, versionLevel) = versionSplitter.Split(item, Constants.AnyProjectSelector);
                        modified = BumpVersionMapWithPatern(versionsMap, pattern, versionLevel) || modified;
                    }
                }

                if (modified)
                {
                    File.WriteAllLines(file, versionsMap.Select(d => d.Key == Constants.AnyProjectSelector ? d.Value : $"{d.Key}{Constants.ProjectVersionSeparator} {d.Value}").ToArray());
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
                while (++level <  versionsPart.Length)
                {
                    versionsPart[level] = "0";
                }
                version = string.Join('.', versionsPart);
            }
            return version;
        }
    }
}
