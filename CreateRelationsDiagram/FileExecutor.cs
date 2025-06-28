namespace CreateRelationsDiagram
{
    /// <summary>
    /// Execute actions over paths and files.
    /// Provides methods to initialize file and directory paths and execute actions on them.
    /// </summary>
    internal class FileExecutor : IFileExecutor
    {
        private string[] files;
        private string[] dirs;
        private bool isInitialized;

        /// <summary>
        /// Initializes the file executor with the specified root path, file pattern, and path pattern.
        /// Retrieves all matching files and directories recursively.
        /// </summary>
        /// <param name="rootPath">The root directory path to search.</param>
        /// <param name="filePattern">The file search pattern (default is "*.*").</param>
        /// <param name="pathPattern">The directory search pattern (default is "*.*").</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rootPath"/> is null.</exception>
        public void Initialize(string rootPath, string filePattern = "*.*", string pathPattern = "*.*")
        {
            rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            files = Directory.GetFiles(rootPath, filePattern, SearchOption.AllDirectories);
            dirs = Directory.GetDirectories(rootPath, pathPattern, SearchOption.AllDirectories);
            isInitialized = true;
        }

        /// <summary>
        /// Executes the specified action on each file retrieved during initialization.
        /// </summary>
        /// <param name="actionForFiles">The action to execute on each file.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actionForFiles"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the executor has not been initialized.</exception>
        public void RunOnFiles(Action<string> actionForFiles)
        {
            if (!isInitialized)
            {
                throw new InvalidOperationException("FileExecutor is not initialized. Call Initialize method first.");
            }

            actionForFiles = actionForFiles ?? throw new ArgumentNullException(nameof(actionForFiles));
            foreach (var item in files)
            {
                actionForFiles.Invoke(item);
            }
        }

        /// <summary>
        /// Executes the specified action on each directory path retrieved during initialization.
        /// </summary>
        /// <param name="actionForPaths">The action to execute on each directory path.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actionForPaths"/> is null.</exception>
        public void RunOnPaths(Action<string> actionForPaths)
        {
            actionForPaths = actionForPaths ?? throw new ArgumentNullException(nameof(actionForPaths));
            foreach (var item in dirs)
            {
                actionForPaths.Invoke(item);
            }
        }
    }
}
