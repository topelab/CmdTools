namespace CmdTools.Contracts
{
    public interface IRelationsGetter
    {
        string Get(IReferencesBag references, IEnumerable<string> excludedElements, string elementFilter);
    }
}