namespace UpdateVersion
{
    internal interface IProjectUpdater
    {
        void Update(string file, string version);
    }
}