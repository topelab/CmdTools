using Avalonia.Controls;
using SplitPDFWin;
using SplitPDFWin.ChangeListeners;
using SplitPDFWin.Factories;
using SplitPDFWin.Managers;
using SplitPDFWin.ViewModels;
using SplitPDFWin.Views;
using Topelab.Core.Resolver.Entities;

namespace CalendarCreator
{
    internal class SetupDI
    {
        internal static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddTransient<Control, MainWindow>(nameof(MainWindowViewModel))
                .AddTransient(typeof(IErrorManager<>), typeof(ErrorManager<>))
                .AddTransient<IMainWindowFactory, MainWindowFactory>()
                .AddTransient<IMainWindowVMChangeListener, MainWindowVMChangeListener>()
                .AddTransient<ISelectFileCommandFactory, SelectFileCommandFactory>()
                .AddTransient<IStartCommandFactory, StartCommandFactory>()
                .AddSingleton<IFilesService, FilesService>()
                .AddFactory(r => App.StorageProvider)
                ;
        }
    }
}