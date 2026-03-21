namespace CmdTools.Shared
{
    using System.ComponentModel;

    public enum Layout
    {
        [Description("dagre")]
        Hierarchical,
        [Description("elk")]
        Adaptive,
    }
}
