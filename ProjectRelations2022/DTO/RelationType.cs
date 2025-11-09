namespace ProjectRelations2022.DTO
{
    using System.ComponentModel;

    public enum RelationType
    {
        [Description("Using")]
        Using,
        [Description("Used By")]
        UsedBy
    }
}
