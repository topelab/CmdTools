namespace ProjectRelations2026.Services
{
    using Topelab.Core.Resolver.Entities;

    internal static class SetupDI
    {
        public static ResolveInfoCollection GetResolveInfoCollection()
        {
            return new ResolveInfoCollection()
                .AddCollection(RelationsShared.SetupDI.Register())
                .AddTransient<IRelationsContextFactory, RelationsContextFactory>()
                ;
        }
    }
}
