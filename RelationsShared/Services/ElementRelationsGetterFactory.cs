namespace RelationsShared.Services
{
    using CmdTools.Contracts;
    using Topelab.Core.Resolver.Interfaces;

    internal class ElementRelationsGetterFactory(IResolver resolver) : IElementRelationsGetterFactory
    {
        public IElementRelationsGetter Create(FinderType finderType)
        {
            return resolver.Get<IElementRelationsGetter>(finderType.ToString());
        }
    }
}
