namespace RelationsShared.Services
{
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using RelationsShared.DTO;

    public interface IOutputRender
    {
        string Create(IEnumerable<Relation> relations, Options options, string pinnedElement = null);
        string Render(string input, UserSettings userSettings);
    }
}