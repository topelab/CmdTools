namespace RelationsShared.Services
{
    using RelationsShared.DTO;

    public interface IRelationsContextInitializer
    {
        void Initialize(RelationsContext context);
    }
}
