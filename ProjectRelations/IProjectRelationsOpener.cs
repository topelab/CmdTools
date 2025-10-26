namespace ProjectRelations
{
    internal interface IProjectRelationsOpener
    {
        void OpenUsedByProject(string projectPath, string projectName);
        void OpenUsingProject(string solutionPah, string projectName);
    }
}