namespace UpdateVersion
{
    internal interface IProjectUpdater
    {
        void Run(string basePath, string version);
    }
}