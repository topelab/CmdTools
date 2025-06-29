namespace CmdTools.Contracts
{
    /// <summary>
    /// Define un contrato para encontrar elementos utilizando las opciones especificadas.
    /// </summary>
    /// <typeparam name="T">Tipo de las opciones utilizadas para la búsqueda de elementos.</typeparam>
    public interface IElementFinder<T>
    {
        /// <summary>
        /// Ejecuta la lógica de búsqueda de elementos utilizando las opciones proporcionadas.
        /// </summary>
        /// <param name="options">Opciones que configuran la búsqueda de elementos.</param>
        void Run(T options);
    }
}
