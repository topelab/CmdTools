namespace RelationsShared.DTO
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
