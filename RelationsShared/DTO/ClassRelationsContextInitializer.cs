namespace RelationsShared.DTO
{
    using CmdTools.Shared;
    using RelationsShared.Services;

    internal class ClassRelationsContextInitializer : IRelationsContextInitializer
    {
        public void Initialize(RelationsContext relationsContext)
        {
            var context = relationsContext as ClassRelationsContext;
            var options = context.Options as ClassOptions;

        }
    }
}
