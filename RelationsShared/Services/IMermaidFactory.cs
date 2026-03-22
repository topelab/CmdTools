namespace RelationsShared.Services
{
    using CmdTools.Shared;
    using RelationsShared.DTO;

    public interface IMermaidFactory
    {
        string Create(IEnumerable<MermaidRelation> relations, Theme theme, Layout layout, Direction direction, string pinnedElement = null);
        string Render(string mmd, UserSettings userSettings);
    }
}