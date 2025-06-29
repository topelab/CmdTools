using CmdTools.Contracts;
using Microsoft.CSharp;
using System.CodeDom;
using System.Reflection;

namespace CreateRelationsDiagram
{
    internal class ClassesFinder : IElementFinder<Options>
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
            var outputFile = options.OutputFile ?? Constants.RelationsFileName;
            if (string.IsNullOrEmpty(assembly))
            {
                Console.WriteLine("Assembly (-a --assembly) is mandatory when classes option is set");
                return;
            }

            Dictionary<string, HashSet<string>> classes = GetClasses(assembly, nameSpace);
            var relationsGetter = relationGetterFactory.Create(options.FinderType);

            var content = relationsGetter.Get(
                classes,
                options.Exclude ?? [],
                options.ProjectFilter);

            Finalize(content, outputFile);
        }

        protected void Finalize(string content, string outputFile)
        {
            File.WriteAllText(outputFile, content);
            Console.WriteLine($"References diagram created at: {outputFile}");
        }

        protected virtual Dictionary<string, HashSet<string>> GetClasses(string assembly, string nameSpace)
        {
            Dictionary<string, HashSet<string>> result = [];
            try
            {
                var types = GetTypesFromAssembly(assembly, nameSpace);

                foreach (var type in types)
                {
                    var typeName = GetFriendlyTypeName(type, nameSpace);
                    if (!result.ContainsKey(typeName))
                    {
                        result[typeName] = GetProperties(nameSpace, type);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading assembly: {ex.Message}");
            }
            return result;
        }

        protected HashSet<string> GetProperties(string nameSpace, Type type)
        {
            HashSet<string> properties = [];
            foreach (var item in type.GetProperties().Where(p => CanGet(p.PropertyType)))
            {
                properties.Add(GetFriendlyTypeName(item.PropertyType, nameSpace));
            }
            return properties;
        }


        protected IEnumerable<Type> GetTypesFromAssembly(string assemblyPath, string nameSpace = null)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            var types = assembly.GetTypes().Where(t => !t.Name.StartsWith('<') && (nameSpace == null || t.Namespace.StartsWith(nameSpace)));
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
                && !type.Name.StartsWith("Expression`");
        }

        protected string GetFriendlyTypeName(Type t, string nameSpace)
        {
            string typeName;
            using (CSharpCodeProvider provider = new())
            {
                CodeTypeReference typeRef = new(t);
                typeName = provider.GetTypeOutput(typeRef);

                if (!string.IsNullOrWhiteSpace(nameSpace))
                {
                    var supNameSpace = nameSpace.Replace(nameSpace.Split('.').Last(), string.Empty);
                    if (typeName.StartsWith(supNameSpace))
                    {
                        typeName = typeName.Replace(typeName.Replace(typeName.Split('.').Last(), string.Empty), string.Empty);
                    }
                    string nameSpaceWithDot = $"{nameSpace}.";
                    if (typeName.Contains(nameSpaceWithDot))
                    {
                        typeName = typeName.Replace(nameSpaceWithDot, string.Empty);
                    }
                    if (typeName.Contains(nameSpace))
                    {
                        typeName = typeName.Replace(nameSpace, string.Empty);
                    }
                }

                typeName = typeName
                    .Replace("System.Collections.Generic.", string.Empty)
                    .Replace("System.Linq.Expressions.", string.Empty)
                    .Replace("System.Linq.", string.Empty)
                    .Replace("System.", string.Empty);

                if (typeName.Contains('<') || typeName.Contains('>'))
                {
                    string originalTypeName = typeName.Replace("<", "&lt;").Replace(">", "&gt");
                    typeName = typeName.Replace("<", "/")
                        .Replace(">", "\\")
                        .Replace(" ", string.Empty);

                    typeName = string.Concat(typeName, "[", originalTypeName, "]");
                }
            }
            return typeName;
        }

    }
}
