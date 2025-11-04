namespace ProjectRelations2022.Services
{
    using Topelab.Core.Resolver.Entities;

    internal static class SetupDI
    {
        public static ResolveInfoCollection GetResolveInfoCollection()
        {
            return new ResolveInfoCollection()
                .AddCollection(CreateRelationsDiagram.SetupDI.Register())
                .AddSingleton<IJsonSettings, JsonSettings>()
                .AddSingleton<IUserSettingsFactory, UserSettingsFactory>()
                .AddTransient<IProjectRelationsOpener, ProjectRelationsOpener>();
        }
    }
}
