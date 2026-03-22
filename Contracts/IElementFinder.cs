namespace CmdTools.Contracts
{
    /// <summary>
    /// Define un contrato para encontrar elementos utilizando las opciones especificadas.
    /// </summary>
    public interface IElementFinder
    {
        string Get<T>(T args) where T : class;

        /// <summary>
        /// Ejecuta la lógica de búsqueda de elementos utilizando las opciones proporcionadas.
        /// </summary>
        /// <param name="options">Opciones que configuran la búsqueda de elementos.</param>
        void Run<T>(T options) where T : class;
    }
}
