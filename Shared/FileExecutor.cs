namespace CmdTools.Shared
{
    using CmdTools.Contracts;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Ejecuta acciones sobre rutas y archivos.
    /// Proporciona métodos para inicializar rutas de archivos y directorios y ejecutar acciones sobre ellos.
    /// </summary>
    internal class FileExecutor : IFileExecutor
    {
        private string[] files;
        private string[] dirs;
        private bool isInitialized;

        /// <summary>
        /// Inicializa el ejecutor de archivos con la ruta raíz, el patrón de archivos y el patrón de rutas especificados.
        /// Recupera todos los archivos y directorios que coinciden de forma recursiva.
        /// </summary>
        /// <param name="rootPath">Ruta raíz donde buscar.</param>
        /// <param name="filePattern">Patrón de búsqueda para archivos (por defecto "*.*").</param>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="rootPath"/> es null.</exception>
        public void Initialize(string rootPath, string filePattern = "*.*", Regex exclusionRegex = null)
        {
            rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            files = [.. Directory.GetFiles(rootPath, filePattern, SearchOption.AllDirectories).Where(f => exclusionRegex is null || !exclusionRegex.IsMatch(f))];
            dirs = [.. Directory.GetDirectories(rootPath, "*.*", SearchOption.AllDirectories).Where(f => exclusionRegex is null || !exclusionRegex.IsMatch(f))];
            isInitialized = true;
        }

        /// <summary>
        /// Ejecuta la acción especificada sobre cada archivo recuperado durante la inicialización.
        /// </summary>
        /// <param name="actionForFiles">Acción a ejecutar sobre cada archivo.</param>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="actionForFiles"/> es null.</exception>
        /// <exception cref="InvalidOperationException">Se lanza si el ejecutor no ha sido inicializado.</exception>
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
        /// Ejecuta la acción especificada sobre cada ruta de directorio recuperada durante la inicialización.
        /// </summary>
        /// <param name="actionForPaths">Acción a ejecutar sobre cada ruta de directorio.</param>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="actionForPaths"/> es null.</exception>
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
