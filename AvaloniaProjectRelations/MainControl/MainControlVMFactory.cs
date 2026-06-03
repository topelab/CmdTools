namespace AvaloniaProjectRelations.MainControl
{
    using CmdTools.Shared;
    using CommandLine;
    using ReactiveUI;
    using RelationsShared.DTO;
    using RelationsShared.Services;
    using System;

    internal class MainControlVMFactory(IUserSettingsFactory userSettingsFactory,
                                        IMainControlVMInitializer mainControlVMInitializer,
                                        IMainControlVMChangeListener mainControlVMChangeListener) : IMainControlVMFactory
    {
        private readonly IUserSettingsFactory userSettingsFactory = userSettingsFactory;
        private readonly IMainControlVMInitializer mainControlVMInitializer = mainControlVMInitializer;
        private readonly IMainControlVMChangeListener mainControlVMChangeListener = mainControlVMChangeListener;
        private UserSettings userSettings;

        private UserSettings UserSettings => userSettings ??= userSettingsFactory.Create(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

        public MainControlVM Create(string[] args)
        {
            var options = new ProjectOptions
            {
                Direction = Direction.LeftToRight,
                Theme = UserSettings.HasTheme ? Enum.Parse<Theme>(UserSettings.Theme) : Theme.Dark,
                Layout = Layout.Adaptive,
            };

            bool isUsedBy = UserSettings.IsUsedBy;
            var result = Parser.Default.ParseArguments<MainControlArguments>(args)
                .WithParsed(o =>
                {
                    options.RootPath = o.RootPath ?? GetArgument(args, 1) ?? Environment.CurrentDirectory;
                    options.WithPackages = o.WithPackages ?? UserSettings.ShowPackages;
                    options.SelectedElement = o.SelectedProject ?? GetArgument(args, 2);
                    options.Exclude = o.Exclude ?? UserSettings.ExcludeProjects;
                    options.RenderType = o.ShowDiagram ? RenderType.Mermaid : RenderType.Text;
                    isUsedBy = o.IsUsedBy;
                });

            options.InitialPath = TryFindInitialPath(options.RootPath);

            var vm = new MainControlVM(options) { Title = App.MainTitle };
            mainControlVMInitializer.Initialize(vm, true);

            if (options.SelectedElement != null)
            {
                var selectedProject = Path.GetFileName(options.SelectedElement).Replace(".csproj", string.Empty, StringComparison.CurrentCultureIgnoreCase);
                vm.SelectedItem = vm.Projects.FirstOrDefault(p => p.Equals(selectedProject, StringComparison.CurrentCultureIgnoreCase));
            }

            vm.SelectedItem ??= vm.Projects.FirstOrDefault();
            vm.IsUsedBy = isUsedBy;
            mainControlVMChangeListener.Start(vm);
            vm.RaisePropertyChanged(nameof(vm.SelectedItem));

            return vm;
        }

        private string TryFindInitialPath(string rootPath)
        {
            string currentPath = rootPath;
            string solutionPath = rootPath;

            if (!IsSolutionPath(currentPath))
            {
                currentPath = Path.GetFullPath(currentPath);
                if (!IsSolutionPath(currentPath))
                {
                    solutionPath = rootPath;
                }
                else
                {
                    solutionPath = currentPath;
                }
            }

            return solutionPath;
        }

        private bool IsSolutionPath(string path)
        {
            var solutionPaths = Directory.EnumerateFiles(path, "*.sln*");
            return solutionPaths.Any();
        }

        private string GetArgument(string[] args, int index)
        {
            if (index >= 0 && index < args.Length)
            {
                return args[index];
            }
            return null;
        }
    }
}
