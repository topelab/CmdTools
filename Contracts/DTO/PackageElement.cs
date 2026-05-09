namespace CmdTools.Contracts.DTO
{
    public record PackageElement(string Name, string Version, string Icon) : ElementRelation(Name, ProjectRelationType.PackageReference)
    {
        public override string ToString()
        {
            return $"{Name}-{Version}:::pkg";
        }

        public string ToString(bool withIcon)
        {
            return withIcon && !string.IsNullOrEmpty(Icon) ? $"{Icon} {Name}-{Version}" : ToString();
        }
    }
}
