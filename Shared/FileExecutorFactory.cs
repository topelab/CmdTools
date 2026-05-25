namespace CmdTools.Shared
{
    using CmdTools.Contracts;
    using System.Text.RegularExpressions;

    internal class FileExecutorFactory : IFileExecutorFactory
    {
        public IFileExecutor Create(string rootPath, string filePattern = "*.*", Regex exclusionRegex = null)
        {
            var executor = new FileExecutor();
            executor.Initialize(rootPath, filePattern, exclusionRegex);
            return executor;
        }
    }
}
