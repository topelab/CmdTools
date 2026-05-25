namespace CmdTools.Contracts
{
    /// <summary>
    /// Define métodos para inicializar y ejecutar acciones sobre archivos y rutas.
    /// </summary>
    public interface IFileExecutor
    {
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
