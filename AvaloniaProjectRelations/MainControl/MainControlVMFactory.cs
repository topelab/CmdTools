namespace AvaloniaProjectRelations.MainControl
{
    using CmdTools.Shared;
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
                RootPath = args.Length > 1 ? args[1] : Environment.CurrentDirectory,
                WithPackages = UserSettings.ShowPackages,
                Direction = Direction.LeftToRight,
                Theme = UserSettings.HasTheme ? Enum.Parse<Theme>(UserSettings.Theme) : Theme.Dark,
                Layout = Layout.Adaptive,
                Exclude = UserSettings.ExcludeProjects,
                RenderType = RenderType.Text,
            };

            options.InitialPath = TryFindInitialPath(options.RootPath);

            var vm = new MainControlVM(options) { Title = App.MainTitle };
            mainControlVMInitializer.Initialize(vm, true);

            if (args.Length > 2)
            {
                var selectedProject = Path.GetFileName(args[2]).Replace(".csproj", string.Empty, StringComparison.CurrentCultureIgnoreCase);
                vm.SelectedItem = vm.Projects.FirstOrDefault(p => p.Equals(selectedProject, StringComparison.CurrentCultureIgnoreCase));
            }

            vm.SelectedItem ??= vm.Projects.FirstOrDefault();
            vm.IsUsedBy = UserSettings.IsUsedBy;
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
    }
}
