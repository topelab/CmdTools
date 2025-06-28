using CommandLine;
using Topelab.Core.Resolver.Microsoft;

namespace CreateRelationsDiagram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Proceed);
        }

        private static void Proceed(Options options)
        {
            var resolver = ResolverFactory.Create(SetupDI.Register());
            var projectFinder = resolver.Get<IProjectFinder>(options.Reverse.ToString());
            projectFinder.Run(options);
        }
    }
}
