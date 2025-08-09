namespace CmdTools.Contracts
{
    public interface IRelationGetterFactory
    {
        IRelationsGetter Create(FinderType finderType);
    }
}