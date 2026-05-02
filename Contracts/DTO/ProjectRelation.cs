namespace CmdTools.Contracts.DTO
{
    public record ProjectRelation(ProjectRelationType RelationType, string Element, string AdditionalInfo) : ElementRelation<ProjectRelationType>(RelationType, Element);
}
