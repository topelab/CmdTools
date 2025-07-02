namespace CmdTools.Shared
{
    using CmdTools.Contracts;
    using Topelab.Core.Resolver.Entities;

    public class SharedSetupDI
    {
        public static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddTransient<IFileExecutor, FileExecutor>()
                .AddTransient<IRelationGetterFactory, RelationGetterFactory>()
                .AddTransient<IRelationsGetter, RelationsGetter>(nameof(FinderType.Projects))
                .AddTransient<IRelationsGetter, RelationsGetter>(nameof(FinderType.Classes))
                .AddTransient<IRelationsGetter, ReverseRelationsGetter>(nameof(FinderType.ReverseProjects))
                .AddTransient<IRelationsGetter, ReverseRelationsGetter>(nameof(FinderType.ReverseClasses))
                ;
        }
    }
}