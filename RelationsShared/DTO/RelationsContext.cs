namespace RelationsShared.DTO
{
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class RelationsContext
    {
        public Options Options { get; set; }
        public ElementsRelations ElementsRelations { get; set; } = [];
        public HashSet<string> Elements { get; set; } = [];
        public Regex ExcludeElements { get; internal set; }
    }
}
