namespace RelationsShared.Services
{
    using RelationsShared.DTO;

    public interface IUserSettingsFactory
    {
        UserSettings Create(string name);
    }
}