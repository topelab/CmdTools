namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using CmdTools.Shared;
    using CommandLine;
    using RelationsShared.Services;
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
            var elementRelationsGetter = resolver.Get<IElementRelationsGetter>(options.FinderType.ToString());
            var outputRenderFactory = resolver.Get<IOutputRenderFactory>();
            var elementRelationWriter = resolver.Get<IElementRelationWriter>();

            var relations = elementRelationsGetter.Get(options);
            var outputRender = outputRenderFactory.Create(options.RenderType);

            var content = outputRender.Create(relations, options);
            elementRelationWriter.Write(content, options.OutputFile, options.OpenOutput);
        }
    }
}
