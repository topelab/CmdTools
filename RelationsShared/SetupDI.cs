namespace RelationsShared
{
    using CmdTools.Contracts;
    using CmdTools.Shared;
    using RelationsShared.Finders;
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
                .AddTransient<IElementFinder, ProjectFinder>(nameof(FinderType.Projects))
                .AddTransient<IElementFinder, ProjectFinder>(nameof(FinderType.ReverseProjects))
                .AddTransient<IElementFinder, ClassesFinder>(nameof(FinderType.Classes))
                .AddTransient<IElementFinder, ClassesFinder>(nameof(FinderType.ReverseClasses))
                .AddTransient<IOutputRender, MermaidRender>(nameof(RenderType.Mermaid))
                .AddTransient<IOutputRender, TextRender>(nameof(RenderType.Text))
                .AddTransient<IOutputRenderFactory, OutputRenderFactory>()
                ;
        }
    }
}
