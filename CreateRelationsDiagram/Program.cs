namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using CmdTools.Shared;
    using CommandLine;
    using RelationsShared.DTO;
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
            RelationsContext context  = options.FinderType switch
            {
                FinderType.Projects => new ProjectRelationsContext(),
                FinderType.Classes => new ClassRelationsContext(),
                _ => throw new NotSupportedException($"Finder type {options.FinderType} is not supported.")
            };
            context.Options = options;
            var relationsContextInitializer = resolver.Get<IRelationsContextInitializer>(options.FinderType.ToString());
            relationsContextInitializer.Initialize(context);

            var relations = elementRelationsGetter.GetFromContext(context);
            var outputRender = outputRenderFactory.Create(options.RenderType);

            var content = outputRender.Create(relations, options);
            elementRelationWriter.Write(content, options.OutputFile, options.OpenOutput);
        }
    }
}
