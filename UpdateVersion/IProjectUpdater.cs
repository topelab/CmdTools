namespace UpdateVersion
{
    internal interface IProjectUpdater
    {
        void Update<T>(string file, T context) where T : class;
    }
}