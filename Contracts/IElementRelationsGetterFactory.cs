namespace CmdTools.Contracts
{
    public interface IElementRelationsGetterFactory
    {
        IElementRelationsGetter Create(FinderType finderType);
    }
}
