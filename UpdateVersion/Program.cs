namespace UpdateVersion
{
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
            var projectExecutor = resolver.Get<IProjectExecutor>();

            projectExecutor.Run(options.Resolve());
        }
    }
}