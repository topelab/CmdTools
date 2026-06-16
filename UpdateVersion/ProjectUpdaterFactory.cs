namespace UpdateVersion
{
    using System;

    internal class ProjectUpdaterFactory : IProjectUpdaterFactory
    {
        public IProjectUpdater Create(ProjectUpdaterType projectUpdaterType)
        {
            return projectUpdaterType switch
            {
                ProjectUpdaterType.Projects => new ProjectUpdater(),
                ProjectUpdaterType.DirectoryBuildProperties => new DirectoryBuildPropertiesUpdater(),
                _ => throw new ArgumentOutOfRangeException(nameof(projectUpdaterType), projectUpdaterType, null)
            };
        }
    }
}
