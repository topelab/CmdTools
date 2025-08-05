namespace UpdateVersion
{
    using CmdTools.Contracts;
    using CommandLine;
    using Topelab.Core.Resolver.Microsoft;

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
            var projectUpdater = resolver.Get<IElementFinder>();

            projectUpdater.Run(options.Resolve());
        }
    }
}