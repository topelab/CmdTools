using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace SplitPDFWin.Managers
{
    internal class FilesService : IFilesService
    {
        private readonly IStorageProvider storageProvider;

        public FilesService(IStorageProvider storageProvider)
        {
            this.storageProvider = storageProvider ?? throw new System.ArgumentNullException(nameof(storageProvider));
        }

        public async Task<IStorageFile> OpenFileAsync(string title, string pattern)
        {
            var fileTypes = new FilePickerFileType(title) { Patterns = [pattern] };
            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = title,
                FileTypeFilter = [fileTypes],
                AllowMultiple = false
            });

            return files?.Count >= 1 ? files[0] : null;
        }

        public async Task<IStorageFile> SaveFileAsync()
        {
            return await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Guardar fichero"
            });
        }

    }
}
