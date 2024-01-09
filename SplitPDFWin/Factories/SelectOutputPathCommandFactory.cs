using Avalonia.Platform.Storage;
using SplitPDFWin.ViewModels;

namespace SplitPDFWin.Factories
{
    internal class SelectOutputPathCommandFactory : CommandFactory<MainWindowViewModel>, ISelectOutputPathCommandFactory
    {
        public override async void Execute()
        {
            var folder = await App.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions() { AllowMultiple = false, Title = "Escoger directorio" });
            if (folder != null && folder.Count > 0)
            {
                context.PdfOutput = folder[0].TryGetLocalPath();
            }
        }
    }
}
