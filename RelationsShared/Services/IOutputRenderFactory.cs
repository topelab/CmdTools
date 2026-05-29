namespace RelationsShared.Services
{
    using CmdTools.Shared;

    public interface IOutputRenderFactory
    {
        IOutputRender Create(RenderType renderType);
    }
}
