namespace CmdTools.Contracts
{
    public interface IElementRelationsInitializer<in TContext>
    {
        void Initialize(TContext context);
    }
}
