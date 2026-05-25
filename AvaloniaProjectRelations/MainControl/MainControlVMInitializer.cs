namespace AvaloniaProjectRelations.MainControl
{
    using AvaloniaProjectRelations.Browser;
    using CmdTools.Shared;
    using RelationsShared.DTO;
    using RelationsShared.Services;

    internal class MainControlVMInitializer(IElementRelationsGetterFactory elementRelationsGetterFactory,
                                            IHtmlViewerVMFactory htmlViewerVMFactory,
                                            IUserSettingsFactory userSettingsFactory,
                                            IOutputRenderFactory outputRenderFactory,
                                            IProjectsServiceFactory projectsServiceFactory,
                                            IProjectRelationsContextInitializer relationsContextInitializer) : IMainControlVMInitializer
    {
        private readonly IElementRelationsGetterFactory elementRelationsGetterFactory = elementRelationsGetterFactory;
        private readonly IHtmlViewerVMFactory htmlViewerVMFactory = htmlViewerVMFactory;
        private readonly IUserSettingsFactory userSettingsFactory = userSettingsFactory;
        private readonly IOutputRenderFactory outputRenderFactory = outputRenderFactory;
        private readonly IProjectsServiceFactory projectsServiceFactory = projectsServiceFactory;
        private readonly IProjectRelationsContextInitializer relationsContextInitializer = relationsContextInitializer;

        private UserSettings UserSettings => field ??= userSettingsFactory.Create(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
        private readonly ProjectRelationsContext context = new();

        public void Initialize(MainControlVM vm, bool isFirstInitialization = false)
        {
            var options = vm.Model;
            context.Options = options;

            if (isFirstInitialization)
            {
                relationsContextInitializer.Initialize(context);
                var htmlViewerVM = htmlViewerVMFactory.Create(string.Empty);
                vm.HtmlViewerVM = htmlViewerVM;
                vm.UserSettings = UserSettings;
                vm.IsUsedBy = true;
                vm.IsUsing = !vm.IsUsedBy;
                vm.ShowListOnly = options.RenderType == RenderType.Text;
                vm.IncludePackages = options.WithPackages;
                InitializeProjects(vm, options);
            }
            else
            {
                var elementRelationsGetter = elementRelationsGetterFactory.Create(options.FinderType);
                var relations = elementRelationsGetter.GetFromContext(context);
                var outputRender = outputRenderFactory.Create(options.RenderType);
                var content = outputRender.Create(relations, options);
                var html = outputRender.RenderToHtml(content, UserSettings);
                vm.HtmlViewerVM.Content = html;
            }
        }

        private void InitializeProjects(MainControlVM vm, ProjectOptions options)
        {
            vm.ProjectData = projectsServiceFactory.Create(options.RootPath, options.ProjectPaths);
            vm.SolutionPath = options.RootPath;
            vm.Projects.Clear();
            foreach (var projectName in vm.ProjectData.GetAllProjectNames())
            {
                vm.Projects.Add(projectName);
            }
        }
    }
}
