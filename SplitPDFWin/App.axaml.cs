using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using CalendarCreator;
using SplitPDFWin.Factories;
using SplitPDFWin.ViewModels;

namespace SplitPDFWin
{
    public partial class App : Application
    {
        public static IStorageProvider StorageProvider { get; private set; }

        public override void Initialize()
        {
            if (!Design.IsDesignMode)
            {
                Create(SetupDI.Register());
            }
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (Design.IsDesignMode)
            {
                return;
            }

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = (Window)Resolve<Control>(nameof(MainWindowViewModel));
                StorageProvider = desktop.MainWindow.StorageProvider;
                desktop.MainWindow.DataContext = Resolve<IMainWindowFactory>().Create();
            }

            base.OnFrameworkInitializationCompleted();
        }

        
    }
}