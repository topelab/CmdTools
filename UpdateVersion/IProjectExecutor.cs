namespace UpdateVersion
{
    internal interface IProjectExecutor
    {
        void Run<T>(T args) where T : class;
    }
}