using Topelab.Core.Resolver.Entities;

namespace CreateRelationsDiagram
{
    internal class SetupDI
    {
        internal static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddTransient<IProjectReferences, ProjectReferences>()
                .AddTransient<IProjectFinder, ProjectFinderReverse>(true.ToString())
                .AddTransient<IProjectFinder, ProjectFinder>(false.ToString())
                .AddTransient<IFileExecutor, FileExecutor>()
                ;
        }
    }
}