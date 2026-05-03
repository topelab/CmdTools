namespace CmdTools.Contracts.DTO
{
    public record ElementRelation<TRelation>(string Parent, TRelation Relation, string Element);
}
