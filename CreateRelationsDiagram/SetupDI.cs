namespace CreateRelationsDiagram
{
    using CmdTools.Shared;
    using RelationsShared.Services;
    using Topelab.Core.Resolver.Entities;

    public class SetupDI
    {
        public static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddCollection(RelationsShared.SetupDI.Register())
                .AddTransient<IOutputRender, SimpleTextRender>(nameof(RenderType.Text))
                ;
        }
    }
}