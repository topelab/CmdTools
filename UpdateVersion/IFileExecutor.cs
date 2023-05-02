using System;

namespace UpdateVersion
{
    /// <summary>
    /// Interface for an implementation that execute actions over paths and files
    /// </summary>
    internal interface IFileExecutor
    {
        /// <summary>
        /// Initialize root path
        /// </summary>
        /// <param name="rootPath">Root path</param>
        /// <param name="filePattern">Optional file pattern</param>
        /// <param name="pathPattern">Optional path pattern</param>
        void Initialize(string rootPath, string filePattern = "*.*", string pathPattern = "*.*");

        /// <summary>
        /// Run action over files
        /// </summary>
        /// <param name="actionForFiles">Action to execute over files</param>
        void RunOnFiles(Action<string> actionForFiles);
        /// <summary>
        /// Run action over paths
        /// </summary>
        /// <param name="actionForPaths">Action to execute over paths</param>
        void RunOnPaths(Action<string> actionForPaths);
    }
}
