using CmdTools.Contracts;
using CmdTools.Shared;
using Topelab.Core.Resolver.Entities;

namespace CreateRelationsDiagram
{
    internal class SetupDI
    {
        internal static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddCollection(SharedSetupDI.Register())
                .AddTransient<IProjectReferences, ProjectReferences>()
                .AddTransient<IElementFinder<Options>, ProjectFinderReverse>(nameof(FinderType.ReverseProjects))
                .AddTransient<IElementFinder<Options>, ProjectFinder>(nameof(FinderType.Projects))
                ;
        }
    }
}