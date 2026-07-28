namespace AvaloniaProjectRelations.MainControl
{
    using CmdTools.Shared;
    using CommandLine;
    using CommandLine.Text;
    using RelationsShared.DTO;
    using RelationsShared.Services;
    using System;
    using Topelab.Core.Avalonia.Services;

    internal class MainControlVMFactory(IUserSettingsFactory userSettingsFactory,
                                        IMainControlVMInitializer mainControlVMInitializer,
                                        IMainControlVMChangeListener mainControlVMChangeListener,
                                        IMessageService messageService) : IMainControlVMFactory
    {
        private readonly IUserSettingsFactory userSettingsFactory = userSettingsFactory;
        private readonly IMainControlVMInitializer mainControlVMInitializer = mainControlVMInitializer;
        private readonly IMainControlVMChangeListener mainControlVMChangeListener = mainControlVMChangeListener;
        private readonly IMessageService messageService = messageService;
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

            bool usedByMe = UserSettings.UsedByMe;

            var result = Parser.Default.ParseArguments<MainControlArguments>(args)
                .WithParsed(o =>
                {
                    options.RootPath = o.RootPath ?? GetArgument(args, 1);
                    options.WithPackages = o.WithPackages ?? UserSettings.ShowPackages;
                    options.SelectedElement = o.SelectedProject ?? GetArgument(args, 2);
                    options.Exclude = o.Exclude ?? UserSettings.ExcludeProjects;
                    options.RenderType = o.ShowDiagram ? RenderType.Mermaid : RenderType.Text;
                    usedByMe = o.UsedByMe;
                });

            options.RootPath ??=  Environment.CurrentDirectory;
            options.InitialPath = TryFindInitialPath(options.RootPath);

            if (args.Contains("--help") || args.Length == 1)
            {
                var parseResult = Parser.Default.ParseArguments<MainControlArguments>(args);
                var helpText = HelpText.AutoBuild(parseResult, h => h, e => e);
                options.HelpText = helpText.ToString();
            }

            var vm = new MainControlVM(options) { Title = App.MainTitle };
            mainControlVMInitializer.Initialize(vm, true);

            if (options.SelectedElement != null)
            {
                var selectedProject = Path.GetFileName(options.SelectedElement).Replace(".csproj", string.Empty, StringComparison.CurrentCultureIgnoreCase);
                vm.SelectedItem = vm.Projects.FirstOrDefault(p => p.Equals(selectedProject, StringComparison.CurrentCultureIgnoreCase));
            }

            vm.SelectedItem ??= vm.Projects.FirstOrDefault();
            vm.UsedByMe = usedByMe;
            mainControlVMChangeListener.Start(vm);
            vm.RaisePropertyChanged(nameof(vm.SelectedItem));

            return vm;
        }

        private string TryFindInitialPath(string rootPath)
        {
            string currentPath = rootPath ?? Environment.CurrentDirectory;
            string solutionPath = rootPath ?? Environment.CurrentDirectory;

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
