namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using System;

    internal class ClassesFinderReverse(IRelationGetterFactory relationGetterFactory) : ClassesFinder(relationGetterFactory)
    {
        public override void Run(Options options)
        {
            var assembly = options.Assembly;
            var nameSpace = options.NameSpace;
            var nameSpaceToClean = options.NameSpace ?? Path.GetFileNameWithoutExtension(options.Assembly);
            var outputFile = options.OutputFile ?? Constants.RelationsFileName;
            var className = options.ClassName;

            if (string.IsNullOrEmpty(assembly))
            {
                Console.WriteLine("Assembly (-a --assembly) is mandatory when classes option is set");
                return;
            }

            var classes = GetClasses(assembly, nameSpace, nameSpaceToClean);
            if (classes.Count > 0)
            {
                var relationsGetter = relationGetterFactory.Create(options.FinderType);
                var content = relationsGetter.Get(
                    classes,
                    options.Exclude ?? [],
                    className);

                content = GetComposition(content, options.Theme, options.Layout, options.Direction);
                Finalize(content, outputFile);
            }
        }
    }
}
