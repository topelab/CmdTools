namespace RelationsShared
{
    using CmdTools.Contracts;
    using CmdTools.Shared;
    using RelationsShared.Services;
    using Topelab.Core.Resolver.Entities;

    public static class SetupDI
    {
        public static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddCollection(SharedSetupDI.Register())
                .AddSingleton<IJsonSettings, JsonSettings>()
                .AddSingleton<IUserSettingsFactory, UserSettingsFactory>()
                .AddTransient<IProjectReferences, ProjectReferences>()
                .AddTransient<IElementRelationsGetter, ProjectRelationsGetter>(nameof(FinderType.Projects))
                .AddTransient<IElementRelationsGetter, ProjectRelationsGetter>(nameof(FinderType.ReverseProjects))
                .AddTransient<IElementRelationsGetter, ClassRelationsGetter>(nameof(FinderType.Classes))
                .AddTransient<IElementRelationsGetter, ClassRelationsGetter>(nameof(FinderType.ReverseClasses))
                .AddTransient<IElementRelationWriter, ElementRelationWriter>()
                .AddTransient<IOutputRender, MermaidRender>(nameof(RenderType.Mermaid))
                .AddTransient<IOutputRender, TextRender>(nameof(RenderType.Text))
                .AddTransient<IOutputRenderFactory, OutputRenderFactory>()
                ;
        }
    }
}
