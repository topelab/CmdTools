namespace RelationsShared.Services
{
    using CmdTools.Shared;
    using Topelab.Core.Resolver.Interfaces;

    internal class OutputRenderFactory(IResolver resolver) : IOutputRenderFactory
    {
        private readonly IResolver resolver = resolver;

        public IOutputRender Create(RenderType renderType)
        {
            return this.resolver.Get<IOutputRender>(renderType.ToString());
        }
    }
}
