namespace RelationsShared.Services
{
    using System.Collections.Generic;
    using System.Linq;

    internal class ProjectsService : IProjectsService
    {
        private readonly Dictionary<string, string> projects = [];

        public ProjectsService(string solutionPath, IEnumerable<string> projectPaths)
        {
            SolutionPath = solutionPath;
            SetProjects(projectPaths);
        }

        public string SolutionPath { get; init; }

        public string GetProjectPath(string projectName)
        {
            string path = null;
            if (projects.TryGetValue(projectName, out var projectPath))
            {
                path = Path.GetDirectoryName(projectPath);
            }
            return path;
        }

        public void AddProject(string projectPath)
        {
            var name = Path.GetFileNameWithoutExtension(projectPath);
            projects[name] = projectPath;
        }

        public void AddProjects(IEnumerable<string> projectPaths)
        {
            foreach (var path in projectPaths)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                projects[name] = path;
            }
        }

        public void SetProjects(IEnumerable<string> projectPaths)
        {
            projects.Clear();
            AddProjects(projectPaths);
        }

        public void ClearProjects()
        {
            projects.Clear();
        }

        public void RemoveProject(string projectName)
        {
            projects.Remove(projectName);
        }

        public IEnumerable<string> GetAllProjectNames()
        {
            return [.. projects.Keys.OrderBy(s => s)];
        }
    }
}
