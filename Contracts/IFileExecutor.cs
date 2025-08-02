namespace CmdTools.Contracts
{
    using System.Text.RegularExpressions;

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
        /// <param name="exclusionRegex">Expresión regular para excluir archivos o rutas específicas. Por defecto es null.</param>
        void Initialize(string rootPath, string filePattern = "*.*", Regex exclusionRegex = null);

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
