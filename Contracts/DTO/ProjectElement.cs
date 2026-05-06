namespace CmdTools.Contracts.DTO
{
    public record ProjectElement(string Name, string Path) : ElementRelation(Name, ProjectRelationType.ProjectReference)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
