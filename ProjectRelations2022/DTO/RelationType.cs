namespace ProjectRelations2022.DTO
{
    using System.ComponentModel;

    internal enum RelationType
    {
        [Description("Using")]
        Using,
        [Description("Used By")]
        UsedBy
    }
}
