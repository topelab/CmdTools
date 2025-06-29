namespace CmdTools.Contracts
{
    public interface IRelationsGetter
    {
        string Get(Dictionary<string, HashSet<string>> references, IEnumerable<string> excludedElements, string elementFilter);
    }
}