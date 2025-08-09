namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using CommandLine;
    using Topelab.Core.Resolver.Microsoft;

    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = ["--help"];
            }

            if (args.Length == 1)
            {
                args = [..args, "--help"];
            }

            Parser.Default.ParseArguments<ProjectOptions, ClassOptions>(args)
                .WithParsed<ProjectOptions>(Proceed)
                .WithParsed<ClassOptions>(Proceed);
        }

        private static void Proceed(Options options)
        {
            var resolver = ResolverFactory.Create(SetupDI.Register());
            var elementFinder = resolver.Get<IElementFinder>(options.FinderType.ToString());
            elementFinder.Run(options);
        }
    }
}
