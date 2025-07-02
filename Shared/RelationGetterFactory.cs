namespace CmdTools.Shared
{
    using CmdTools.Contracts;
    using Topelab.Core.Resolver.Interfaces;

    internal class RelationGetterFactory : IRelationGetterFactory
    {
        private readonly IResolver resolver;

        public RelationGetterFactory(IResolver resolver)
        {
            this.resolver = resolver;
        }

        public IRelationsGetter Create(FinderType finderType)
        {
            return resolver.Get<IRelationsGetter>(finderType.ToString());
        }
    }
}
