namespace CmdTools.Contracts.DTO
{
    using System.Collections.Generic;

    public record ProjectRelations(string ProjectName, IEnumerable<ProjectRelation> Relations) : ElementRelations<IEnumerable<ProjectRelation>>(ProjectName, Relations);
}
