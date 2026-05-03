namespace CmdTools.Contracts.DTO
{
    public record ProjectElement(string Name, string Path) : Element(Name, ProjectRelationType.ProjectReference)
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
