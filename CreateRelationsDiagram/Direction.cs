namespace CreateRelationsDiagram
{
    using System.ComponentModel;

    internal enum Direction
    {
        [Description("TD")]
        TopToDown,
        [Description("LR")]
        LeftToRight,
        [Description("RL")]
        RightToLeft,
        [Description("BT")]
        BottomToTop
    }
}
