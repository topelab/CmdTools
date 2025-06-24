using Topelab.Core.Resolver.Entities;

namespace CreateRelationsDiagram
{
    internal class SetupDI
    {
        internal static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddTransient<IProjectReferences, ProjectReferences>()
                .AddTransient<IProjectFinder, ProjectFinder>()
                .AddTransient<IFileExecutor, FileExecutor>()
                ;
        }
    }
}