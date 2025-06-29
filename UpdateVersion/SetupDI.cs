using CmdTools.Contracts;
using CmdTools.Shared;
using Topelab.Core.Resolver.Entities;

namespace UpdateVersion
{
    internal class SetupDI
    {
        internal static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddCollection(SharedSetupDI.Register())
                .AddTransient<IElementFinder<Options>, ProjectFinder>()
                .AddTransient<IProjectUpdater, ProjectUpdater>()

                .AddSingleton<IVersionSplitter, VersionSplitter>()
                .AddSingleton<IVersionBumper, VersionBumper>()
                ;
        }
    }
}