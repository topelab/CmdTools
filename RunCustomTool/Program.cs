namespace RunCustomTool
{
    using Microsoft.CSharp;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Resources.Tools;

    internal class Program
    {
        static void Main(string[] args)
        {
            var codeProvider = new CSharpCodeProvider();
            string resxFile = args[0];
            string outputPath = Path.GetDirectoryName(resxFile);

            string className = Path.GetFileNameWithoutExtension(resxFile);
            string outputFile = Path.Combine(outputPath, $"{className}.Designer.cs");
            string nameSpace = args[1];

            CodeCompileUnit code = StronglyTypedResourceBuilder.Create(resxFile, className, nameSpace, codeProvider, false, out var unmatchedElements);

            using StreamWriter writer = new StreamWriter(outputFile, false, System.Text.Encoding.UTF8);
            codeProvider.GenerateCodeFromCompileUnit(code, writer, new CodeGeneratorOptions());

            if (unmatchedElements.Length> 0)
            {
                Console.WriteLine($"Unmatched elements:\n -{string.Join("\n - ", unmatchedElements)}");
            }
        }
    }
}
