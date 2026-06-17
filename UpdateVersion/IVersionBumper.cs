namespace UpdateVersion
{
    using System.Collections.Generic;

    internal interface IVersionBumper
    {
        void Bump(string file, IEnumerable<string> versionsToBump, Dictionary<string, string> versionsMap);
        bool TryBump(IEnumerable<string> versionsToBump, Dictionary<string, string> versionsMap);
    }
}