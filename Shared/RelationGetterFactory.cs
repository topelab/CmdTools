namespace CmdTools.Shared
{
    using CmdTools.Contracts;
    using Topelab.Core.Resolver.Interfaces;

    internal class RelationGetterFactory(IResolver resolver) : IRelationGetterFactory
    {
        private readonly IResolver resolver = resolver;

        public IRelationsGetter Create(FinderType finderType)
        {
            return resolver.Get<IRelationsGetter>(finderType.ToString());
        }
    }
}
