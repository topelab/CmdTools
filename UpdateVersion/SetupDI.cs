namespace UpdateVersion
{
    using CmdTools.Contracts;
    using CmdTools.Shared;
    using Topelab.Core.Resolver.Entities;

    internal class SetupDI
    {
        internal static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddCollection(SharedSetupDI.Register())
                .AddTransient<IElementFinder, ProjectFinder>()
                .AddTransient<IProjectUpdater, ProjectUpdater>()

                .AddSingleton<IVersionSplitter, VersionSplitter>()
                .AddSingleton<IVersionBumper, VersionBumper>()
                ;
        }
    }
}