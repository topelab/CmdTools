using ReactiveUI;
using SplitPDFWin.ChangeListeners;
using SplitPDFWin.ViewModels;
using Avalonia.Platform.Storage;

namespace SplitPDFWin.Factories
{
    internal class MainWindowFactory : IMainWindowFactory
    {
        private readonly ISelectFileCommandFactory selectFileCommandFactory;
        private readonly IMainWindowVMChangeListener changeListener;
        private readonly IStartCommandFactory startCommandFactory;

        public MainWindowFactory(ISelectFileCommandFactory selectFileCommandFactory, IMainWindowVMChangeListener changeListener, IStartCommandFactory startCommandFactory)
        {
            this.selectFileCommandFactory = selectFileCommandFactory ?? throw new System.ArgumentNullException(nameof(selectFileCommandFactory));
            this.changeListener = changeListener ?? throw new System.ArgumentNullException(nameof(changeListener));
            this.startCommandFactory = startCommandFactory ?? throw new System.ArgumentNullException(nameof(startCommandFactory));
        }

        public MainWindowViewModel Create()
        {
            MainWindowViewModel viewModel = new MainWindowViewModel();
            viewModel.SelectFileCommand = selectFileCommandFactory.Create(viewModel);
            viewModel.StartCommand = startCommandFactory.Create(viewModel);
            viewModel.SelectOutputPathCommand = ReactiveCommand.Create(async () =>
            {
                var folder = await App.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions() { AllowMultiple = false, Title = "Escoger directorio" });
                if (folder != null && folder.Count > 0)
                {
                    viewModel.PdfOutput = folder[0].TryGetLocalPath();
                }
            });

            changeListener.Start(viewModel);
            return viewModel;
        }
    }
}
