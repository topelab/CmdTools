namespace AvaloniaProjectRelations.MainControl
{
    using CmdTools.Shared;
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

            var vm = new MainControlVM(options) { Title = App.MainTitle };
            mainControlVMInitializer.Initialize(vm, true);
            mainControlVMChangeListener.Start(vm);
            vm.SelectedItem = args.Length > 2 ? vm.Projects.FirstOrDefault(p => p.Equals(args[2], StringComparison.CurrentCultureIgnoreCase)) : vm.Projects.FirstOrDefault();

            return vm;
        }
    }
}
