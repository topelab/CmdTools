namespace CmdTools.Contracts.DTO
{
    public class Relation
    {
        public string Element { get; init; }
        public string Reference { get; init; }
        public int Level { get; set; }

        public Relation()
        {
        }
    }
}
