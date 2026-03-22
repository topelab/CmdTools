namespace CmdTools.Contracts
{
    using CmdTools.Contracts.DTO;

    public interface IRelationsGetter
    {
        IEnumerable<TRelation> Get<TRelation>(IReferencesBag references, string elementFilter) where TRelation : Relation, new();
    }
}