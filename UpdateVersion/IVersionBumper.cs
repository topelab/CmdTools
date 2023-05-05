using System.Collections.Generic;

namespace UpdateVersion
{
    internal interface IVersionBumper
    {
        void Bump(string file, IEnumerable<string> versionsToBump, Dictionary<string, string> versionsMap);
    }
}