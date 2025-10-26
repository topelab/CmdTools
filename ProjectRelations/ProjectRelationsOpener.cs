namespace ProjectRelations
{
    using ProjectRelations.Services;
    using System.IO;

    internal class ProjectRelationsOpener : IProjectRelationsOpener
    {
        private readonly UserSettings userSettings;

        public ProjectRelationsOpener(IUserSettingsFactory userSettingsFactory)
        {
            userSettings = userSettingsFactory.Create();
        }

        public void OpenUsedByProject(string projectPath, string projectName)
        {
            //string outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", $"used-by-{projectName.ToLower()}.mmd");
            string withPackagesFlag = userSettings.ShowPackages ? " -w" : string.Empty;
            Run($"Projects USED BY {projectName}", $"-d LR -s {Path.GetDirectoryName(projectPath)}{withPackagesFlag}");
        }

        public void OpenUsingProject(string solutionPah, string projectName)
        {
            //string outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", $"using-{projectName.ToLower()}.mmd");
            Run($"Projects USING {projectName}", $"-d LR -s {solutionPah} -p {projectName}");
        }

        private void Run(string title, string arguments)
        {
            var tempFile = string.Concat(Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName())), ".mmd");
            string theme = userSettings.HasTheme ? $" -t {userSettings.Theme}" : " -t Dark";

            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo(userSettings.CreateRelationsDiagramPath)
            {
                Arguments = $"{arguments}{theme} -o {tempFile}",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process.Start();
            process.WaitForExit(5000);

            if (!process.HasExited)
            {
                process.Kill();
            }

            var window = new RelationsToolWindow(tempFile, userSettings);
            window.Text = title;
            window.ShowDialog();
        }
    }
}
