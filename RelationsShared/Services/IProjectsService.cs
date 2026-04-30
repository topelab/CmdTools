namespace RelationsShared.Services
{
    public interface IProjectsService
    {
        string SolutionPath { get; }

        void AddProject(string projectPath);
        void AddProjects(IEnumerable<string> projectPaths);
        void ClearProjects();
        IEnumerable<string> GetAllProjectNames();
        string GetProjectPath(string projectName);
        void RemoveProject(string projectName);
        void SetProjects(IEnumerable<string> projectPaths);
    }
}