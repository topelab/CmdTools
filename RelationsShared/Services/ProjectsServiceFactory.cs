namespace RelationsShared.Services
{
    using System.Collections.Generic;

    internal class ProjectsServiceFactory : IProjectsServiceFactory
    {
        public IProjectsService Create(string solutionPath, IEnumerable<string> projectPaths)
        {
            return new ProjectsService(solutionPath, projectPaths);
        }
    }
}
