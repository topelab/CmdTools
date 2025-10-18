namespace ProjectRelations
{
    using System.IO;

    internal class ProjectRelationsOpener
    {
        public void OpenUsedByProject(string projectPath, string projectName)
        {
            //string outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", $"used-by-{projectName.ToLower()}.mmd");
            Run($"-d LR -s {Path.GetDirectoryName(projectPath)}");
        }

        public void OpenUsingProject(string solutionPah, string projectName)
        {
            //string outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", $"using-{projectName.ToLower()}.mmd");
            Run($"-d LR -s {solutionPah} -p {projectName}");
        }

        private void Run(string arguments)
        {
            var tempFile = string.Concat(Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName())), ".mmd");

            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo(@"C:\arc\output\tools\CreateRelationsDiagram.exe")
            {
                Arguments = $"{arguments} -o {tempFile}",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process.Start();
            process.WaitForExit(5000);

            if (!process.HasExited)
            {
                process.Kill();
            }

            var window = new RelationsToolWindow(tempFile);
            window.Text = "Project Relations";
            window.ShowDialog();
        }
    }
}
