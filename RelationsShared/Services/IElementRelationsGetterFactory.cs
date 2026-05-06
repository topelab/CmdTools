using CmdTools.Contracts;

namespace RelationsShared.Services
{
    public interface IElementRelationsGetterFactory
    {
        IElementRelationsGetter Create(FinderType finderType);
    }
}
