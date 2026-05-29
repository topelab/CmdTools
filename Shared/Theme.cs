namespace CmdTools.Shared
{
    using System.ComponentModel;

    public enum Theme
    {
        [Description("default")]
        Default,
        [Description("base")]
        Base,
        [Description("mc")]
        MermaidChart,
        [Description("neo")]
        Neo,
        [Description("neo-dark")]
        NeoDark,
        [Description("forest")]
        Forest,
        [Description("dark")]
        Dark,
        [Description("neutral")]
        Neutral
    }
}