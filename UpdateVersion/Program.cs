using CommandLine;
using System;
using System.Linq;
using Topelab.Core.Resolver.Microsoft;

namespace UpdateVersion
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
            var projectUpdater = resolver.Get<IProjectUpdater>();

            projectUpdater.Run(AppContext.BaseDirectory, options.Versions.FirstOrDefault());
        }
    }
}