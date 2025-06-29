namespace CmdTools.Contracts
{
    /// <summary>
    /// Define métodos para inicializar y ejecutar acciones sobre archivos y rutas.
    /// </summary>
    public interface IFileExecutor
    {
        /// <summary>
        /// Inicializa el ejecutor de archivos con la ruta raíz y los patrones de archivo y ruta especificados.
        /// </summary>
        /// <param name="rootPath">Ruta raíz donde buscar archivos y rutas.</param>
        /// <param name="filePattern">Patrón de búsqueda para archivos. Por defecto es "*.*".</param>
        /// <param name="pathPattern">Patrón de búsqueda para rutas. Por defecto es "*.*".</param>
        void Initialize(string rootPath, string filePattern = "*.*", string pathPattern = "*.*");

        /// <summary>
        /// Ejecuta una acción sobre cada archivo que coincide con el patrón especificado.
        /// </summary>
        /// <param name="actionForFiles">Acción a ejecutar para cada archivo.</param>
        void RunOnFiles(Action<string> actionForFiles);

        /// <summary>
        /// Ejecuta una acción sobre cada ruta que coincide con el patrón especificado.
        /// </summary>
        /// <param name="actionForPaths">Acción a ejecutar para cada ruta.</param>
        void RunOnPaths(Action<string> actionForPaths);
    }
}
