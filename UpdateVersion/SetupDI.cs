namespace UpdateVersion
{
    using CmdTools.Shared;
    using Topelab.Core.Resolver.Entities;

    internal class SetupDI
    {
        internal static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddCollection(SharedSetupDI.Register())
                .AddTransient<IProjectExecutor, ProjectExecutor>()
                .AddSingleton<IVersionSplitter, VersionSplitter>()
                .AddSingleton<IProjectUpdaterFactory, ProjectUpdaterFactory>()
                .AddSingleton<IVersionBumper, VersionBumper>()
                ;
        }
    }
}