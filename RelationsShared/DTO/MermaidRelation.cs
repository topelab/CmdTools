namespace RelationsShared.DTO
{
    using CmdTools.Contracts.DTO;

    public class MermaidRelation : Relation
    {
        public override string ToString()
        {
            return $"\t{Element} -->\t{Reference} ";
        }
    }
}
