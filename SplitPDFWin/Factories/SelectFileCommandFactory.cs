using Avalonia.Platform.Storage;
using SplitPDFWin.Managers;
using SplitPDFWin.ViewModels;
using System;
using System.Threading.Tasks;

namespace SplitPDFWin.Factories
{
    internal class SelectFileCommandFactory(IFilesService filesService) : CommandFactory<MainWindowViewModel>, ISelectFileCommandFactory
    {
        private readonly IFilesService filesService = filesService ?? throw new ArgumentNullException(nameof(filesService));

        public override async void Execute()
        {
            var file = await OpenFileAsync();
            if (file != null)
            {
                context.PdfInput = file.TryGetLocalPath();
            }
        }

        private async Task<IStorageFile> OpenFileAsync()
        {
            IStorageFile file;

            try
            {
                file = await filesService.OpenFileAsync("Abrir PDF", "*.pdf");
            }
            catch (Exception e)
            {
                context.ErrorManager.SetError(nameof(context.PdfInput), e.Message);
                file = null;
            }

            return file;
        }
    }
}
