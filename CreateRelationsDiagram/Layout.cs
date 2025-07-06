namespace CreateRelationsDiagram
{
    using System.ComponentModel;

    internal enum Layout
    {
        [Description("dagre")]
        Hierarchical,
        [Description("elk")]
        Adaptive,
    }
}
