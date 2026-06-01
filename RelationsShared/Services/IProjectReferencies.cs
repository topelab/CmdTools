namespace RelationsShared.Services
{
    using CmdTools.Shared;
    using RelationsShared.DTO;

    internal interface IProjectReferences
    {
        ReferencesBag GetInverseReferences(HashSet<string> projectFiles);
        ReferencesBag GetReferences(HashSet<string> projectFiles);
        void Initialize(ProjectRelationsContext context);
    }
}
