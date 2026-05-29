namespace RelationsShared.Services
{
    using System.Collections.Generic;

    public interface IProjectsServiceFactory
    {
        IProjectsService Create(string solutionPath, IEnumerable<string> projectPaths);
    }
}
