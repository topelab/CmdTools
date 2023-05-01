using System.Collections.Generic;

namespace UpdateVersion
{
    internal interface IProjectFinder
    {
        void Run(string basePath, IEnumerable<string> versions);
    }
}