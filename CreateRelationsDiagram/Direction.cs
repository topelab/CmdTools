namespace CreateRelationsDiagram
{
    using System.ComponentModel;

    internal enum Direction
    {
        [Description("TD")]
        TopToDown,
        [Description("TD")]
        TD,
        [Description("LR")]
        LeftToRight,
        [Description("LR")]
        LR,
        [Description("RL")]
        RightToLeft,
        [Description("RL")]
        RL,
        [Description("BT")]
        BottomToTop,
        [Description("BT")]
        BT,
    }
}
