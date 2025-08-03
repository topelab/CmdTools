namespace CreateRelationsDiagram
{
    using CmdTools.Contracts;
    using CmdTools.Shared;
    using Microsoft.CSharp;
    using System.CodeDom;
    using System.Reflection;
    using System.Text.RegularExpressions;

    internal class ClassesFinder : ElementFinderBase, IElementFinder<Options>
    {
        protected readonly IRelationGetterFactory relationGetterFactory;

        public ClassesFinder(IRelationGetterFactory relationGetterFactory)
        {
            this.relationGetterFactory = relationGetterFactory;
        }

        public void Run(Options options)
        {
            var assembly = options.Assembly;
            var nameSpace = options.NameSpace;
            var nameSpaceToClean = Path.GetFileNameWithoutExtension(options.Assembly);
            var outputFile = options.OutputFile;
            var className = options.ClassName;
            var excludeClasses = string.IsNullOrEmpty(options.Exclude) ? null : new Regex(options.Exclude, RegexOptions.IgnoreCase);

            if (string.IsNullOrEmpty(assembly))
            {
                Console.WriteLine("Assembly (-a --assembly) is mandatory when classes option is set");
                return;
            }

            var classes = GetClasses(assembly, nameSpace, nameSpaceToClean, excludeClasses);
            if (classes.Count > 0)
            {
                var relationsGetter = relationGetterFactory.Create(options.FinderType);

                var content = relationsGetter.Get(
                    classes,
                    className);

                content = GetComposition(content, options.Theme, options.Layout, options.Direction);
                Finalize(content, outputFile);
            }
        }

        protected ReferencesBag GetClasses(string assembly, string nameSpace, string nameSpaceToClean, Regex excludeClasses)
        {
            ReferencesBag result = [];
            try
            {
                var types = GetTypesFromAssembly(assembly, nameSpace);

                foreach (var type in types)
                {
                    var typeName = GetFriendlyTypeName(type, nameSpaceToClean);
                    if ((excludeClasses is null || !excludeClasses.IsMatch(typeName)) && !result.ContainsKey(typeName))
                    {
                        result[typeName] = GetProperties(nameSpaceToClean, type, excludeClasses);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading assembly: {ex.Message}");
            }
            return result;
        }

        protected HashSet<string> GetProperties(string nameSpace, Type type, Regex excludeClasses)
        {
            HashSet<string> properties = [];
            foreach (var item in type.GetProperties().Where(p => CanGet(p.PropertyType)))
            {
                var propertyName = GetFriendlyTypeName(item.PropertyType, nameSpace);
                if (excludeClasses is null || !excludeClasses.IsMatch(propertyName))
                {
                    properties.Add(propertyName);
                }
            }
            return properties;
        }


        protected IEnumerable<Type> GetTypesFromAssembly(string assemblyPath, string nameSpace = null)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            var types = assembly.GetTypes().Where(t => !t.Name.StartsWith('<') && (nameSpace == null || (t.Namespace != null && t.Namespace.StartsWith(nameSpace, StringComparison.CurrentCultureIgnoreCase))));
            return types;
        }

        protected bool CanGet(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            return typeCode == TypeCode.Object
                && (type.IsClass || type.IsInterface)
                && !type.Name.StartsWith("Byte[")
                && !type.Name.StartsWith("Func`")
                && !type.Name.StartsWith("Action`")
                && !type.Name.StartsWith("Expression`")
                && type.Name != typeof(object).Name
                && type.Name != typeof(Type).Name;
        }

        protected string GetFriendlyTypeName(Type t, string nameSpace)
        {
            string typeName;
            using (CSharpCodeProvider provider = new())
            {
                CodeTypeReference typeRef = new(t);
                typeName = provider.GetTypeOutput(typeRef);
                typeName = GetSimplifiedNameSpace(nameSpace, typeName);
                typeName = TryEncode(typeName);
            }
            return typeName;
        }

        private string TryEncode(string typeName)
        {
            if (typeName.Contains('<') || typeName.Contains('>'))
            {
                var originalTypeName = typeName.Replace("<", "&lt;").Replace(">", "&gt");
                typeName = typeName.Replace("<", "/")
                    .Replace(">", "\\")
                    .Replace(" ", string.Empty);

                typeName = string.Concat(typeName, "[", originalTypeName, "]");
            }

            return typeName;
        }

        private string GetSimplifiedNameSpace(string nameSpace, string typeName)
        {
            typeName = typeName
                .Replace("System.Collections.Generic.", string.Empty)
                .Replace("System.Linq.Expressions.", string.Empty)
                .Replace("System.Linq.", string.Empty)
                .Replace("System.", string.Empty);

            if (!string.IsNullOrEmpty(nameSpace))
            {
                var nameSpaceParts = nameSpace.Split('.');
                for (var i = nameSpaceParts.Length; i > 0; i--)
                {
                    var restOfNameSpace = string.Concat(string.Join(".", nameSpaceParts.Take(i)), ".");
                    typeName = typeName.Replace(restOfNameSpace, string.Empty);
                }
            }

            return typeName;
        }
    }
}
