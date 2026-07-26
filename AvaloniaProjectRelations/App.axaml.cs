using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.VisualTree;
using AvaloniaProjectRelations.Main;
using AvaloniaProjectRelations.Resources;
using Microsoft.EntityFrameworkCore;
using RelationsShared.Services;
using System.Globalization;
using System.Reflection;
using Topelab.Core.Avalonia.Base;
using Topelab.Core.Resolver.Interfaces;

namespace AvaloniaProjectRelations
{
    public partial class App : Application
    {
        public static IResolver Resolver { get; private set; }
        internal static MainWindowVM MainVM { get; private set; }
        internal static PlatformThemeVariant ThemeVariant { get; private set; }

        internal static string MainTitle { get; private set; }

        internal static Guid UniqueId { get; private set; }

        public override void Initialize()
        {
            UniqueId = Guid.NewGuid();
            Resolver = Create(SetupDI.Register());
            var uiCulture = ConfigHelper.Config["UICulture"];
            if (uiCulture != null && !uiCulture.Equals("auto", StringComparison.CurrentCultureIgnoreCase))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(uiCulture);
            }

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (Design.IsDesignMode)
            {
                return;
            }

            MainTitle = $"{Strings.MainTitle} v. {GetVersion()}";
            MainVM = Resolve<IMainWindowVMFactory>().Create();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = (Window)Resolve<Control>(nameof(MainWindowVM));
                desktop.MainWindow.DataContext = MainVM;
                desktop.MainWindow.Closing += MainWindow_ClosingAsync;
                var platformSettings = desktop.MainWindow.GetPlatformSettings();
                var colorValues = platformSettings.GetColorValues();
                ThemeVariant = colorValues.ThemeVariant;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void MainWindow_ClosingAsync(object sender, WindowClosingEventArgs e)
        {
            var viewModel = ((Window)sender).DataContext as BaseVM;
            if (CanReplaceCloseWithCancel(viewModel))
            {
                e.Cancel = true;
                viewModel.ContentViewModel.CancelCommand.Execute(null);
            }
            else
            {
                Resolve<IEmbededWebServer>()?.Stop();
            }
        }

        private static bool CanReplaceCloseWithCancel(BaseVM viewModel)
        {
            return viewModel != null
                && viewModel.ContentViewModel != null
                && viewModel.ContentViewModel != App.MainVM.PrincipalVM
                && viewModel.ContentViewModel.CancelCommand != null;
        }

        private static string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}