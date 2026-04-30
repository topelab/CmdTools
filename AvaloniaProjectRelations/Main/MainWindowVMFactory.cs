namespace AvaloniaProjectRelations.Main
{
    using AvaloniaProjectRelations.MainControl;

    internal class MainWindowVMFactory(IMainControlVMFactory mainControlVMFactory) : IMainWindowVMFactory
    {
        private readonly IMainControlVMFactory mainControlVMFactory = mainControlVMFactory;

        public MainWindowVM Create()
        {
            var mainControlVM = mainControlVMFactory.Create(Environment.GetCommandLineArgs());
            MainWindowVM mainWindowVM = new(mainControlVM);
            return mainWindowVM;
        }
    }
}
