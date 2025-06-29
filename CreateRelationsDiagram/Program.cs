using CmdTools.Contracts;
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
            var elementFinder = resolver.Get<IElementFinder<Options>>(options.FinderType.ToString());
            elementFinder.Run(options);
        }
    }
}
