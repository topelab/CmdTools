namespace ProjectRelations.Services
{
    using Topelab.Core.Resolver.Entities;
    using Topelab.Core.Resolver.Interfaces;
    using Topelab.Core.Resolver.Unity;

    internal static class SetupDI
    {
        public static IResolver Resolver { get; private set; } = null!;

        public static void Initialize()
        {
            var resolver = ResolverFactory.Create(
                new ResolveInfoCollection()
                .AddSingleton<IJsonSettings, JsonSettings>()
                .AddSingleton<IUserSettingsFactory, UserSettingsFactory>()
                .AddTransient<IProjectRelationsOpener, ProjectRelationsOpener>()
                );

            Resolver = resolver;
        }
    }
}
