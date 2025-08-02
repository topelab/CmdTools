namespace CmdTools.Contracts
{
    public interface IRelationsGetter
    {
        string Get(IReferencesBag references, string elementFilter);
    }
}