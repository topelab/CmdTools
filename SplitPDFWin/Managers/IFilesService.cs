using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace SplitPDFWin.Managers
{
    internal interface IFilesService
    {
        Task<IStorageFile> OpenFileAsync(string title, string pattern);
        Task<IStorageFile> SaveFileAsync();
    }
}