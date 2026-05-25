namespace CmdTools.Contracts
{
    using System.Text.RegularExpressions;

    public interface IFileExecutorFactory
    {
        /// <summary>
        /// Crea el ejecutor de archivos con la ruta raíz y los patrones de archivo y ruta especificados.
        /// </summary>
        /// <param name="rootPath">Ruta raíz donde buscar archivos y rutas.</param>
        /// <param name="filePattern">Patrón de búsqueda para archivos. Por defecto es "*.*".</param>
        /// <param name="exclusionRegex">Expresión regular para excluir archivos o rutas específicas. Por defecto es null.</param>
        public IFileExecutor Create(string rootPath, string filePattern = "*.*", Regex exclusionRegex = null);
    }
}
