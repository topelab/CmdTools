using System.Text;

namespace CreateRelationsDiagram
{
    internal class ProjectFinder : IProjectFinder
    {
        private readonly IProjectReferences projectReferences;
        private readonly IFileExecutor fileExecutor;

        public ProjectFinder(IProjectReferences projectReferences, IFileExecutor fileExecutor)
        {
            this.projectReferences = projectReferences ?? throw new ArgumentNullException(nameof(projectReferences));
            this.fileExecutor = fileExecutor ?? throw new ArgumentNullException(nameof(fileExecutor));
        }

        public void Run(Options options)
        {
            var path = options.BasePath ?? Environment.ProcessPath;
            var outputFile = options.OutputFile ?? Constants.RelationsFileName;

            StringBuilder content = new StringBuilder("```mermaid\nclassDiagram\n");
            fileExecutor.Initialize(path, Constants.FilePattern);
            fileExecutor.RunOnFiles(file =>
            {
                var references = projectReferences.Get(file);
                foreach (var reference in references)
                {
                    content.AppendLine($"\t{Path.GetFileNameWithoutExtension(file)} --> {reference}");
                }
            });
            content.AppendLine("```");

            if (content.Length > 0)
            {
                File.WriteAllText(outputFile, content.ToString());
                Console.WriteLine($"Project references diagram created at: {outputFile}");
            }
            else
            {
                Console.WriteLine("No project references found.");
            }
        }
    }
}
