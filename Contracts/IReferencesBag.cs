namespace CmdTools.Contracts
{
    public interface IReferencesBag : IDictionary<string, HashSet<string>>
    {
        void AddReference(string element, string reference);
        void AddReferences(string element, IEnumerable<string> references);
    }
}