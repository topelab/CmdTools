namespace RelationsShared.Services
{
    using CmdTools.Contracts.DTO;
    using CmdTools.Shared;
    using RelationsShared.DTO;

    public interface IOutputRender
    {
        string Create(IEnumerable<Relation> relations, Options options);
        string RenderToHtml(string input, UserSettings userSettings);
    }
}