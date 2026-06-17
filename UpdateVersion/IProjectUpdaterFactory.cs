namespace UpdateVersion
{
    internal interface IProjectUpdaterFactory
    {
        IProjectUpdater Create(ProjectUpdaterType projectUpdaterType);
    }
}