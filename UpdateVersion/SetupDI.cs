using Topelab.Core.Resolver.Entities;

namespace UpdateVersion
{
    internal class SetupDI
    {
        internal static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddTransient<IFileExecutor, FileExecutor>()
                .AddTransient<IProjectFinder, ProjectFinder>()
                .AddTransient<IProjectUpdater, ProjectUpdater>()

                .AddSingleton<IVersionSplitter, VersionSplitter>()
                .AddSingleton<IVersionBumper, VersionBumper>()
                ;
        }
    }
}