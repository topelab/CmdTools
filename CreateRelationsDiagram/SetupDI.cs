namespace CreateRelationsDiagram
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
                .AddTransient<IProjectReferences, ProjectReferences>()
                .AddTransient<IElementFinder, ProjectFinder>(nameof(FinderType.Projects))
                .AddTransient<IElementFinder, ProjectFinder>(nameof(FinderType.ReverseProjects))
                .AddTransient<IElementFinder, ClassesFinder>(nameof(FinderType.Classes))
                .AddTransient<IElementFinder, ClassesFinder>(nameof(FinderType.ReverseClasses))
                ;
        }
    }
}