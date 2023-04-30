using System;

namespace UpdateVersion
{
    internal class ProjectUpdater : IProjectUpdater
    {
        private readonly IFileExecutor fileExecutor;

        public ProjectUpdater(IFileExecutor fileExecutor)
        {
            this.fileExecutor = fileExecutor ?? throw new ArgumentNullException(nameof(fileExecutor));
        }

        public void Run(string basePath, string version)
        {
            fileExecutor.Initialize(basePath, "*.csproj");
            fileExecutor.RunOnFiles(file => TryUpdate(file, version));
        }

        private static void TryUpdate(string file, string version)
        {
            Console.WriteLine($"{file} to {version}");
        }
    }
}
