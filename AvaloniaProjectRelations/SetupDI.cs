using Avalonia.Controls;
using AvaloniaProjectRelations.Browser;
using AvaloniaProjectRelations.Commands;
using AvaloniaProjectRelations.Main;
using AvaloniaProjectRelations.MainControl;
using Topelab.Core.Resolver.Entities;

namespace AvaloniaProjectRelations
{
    internal class SetupDI
    {
        internal static ResolveInfoCollection Register()
        {
            return new ResolveInfoCollection()
                .AddCollection(Topelab.Core.Avalonia.SetupDI.Register())
                .AddCollection(RelationsShared.SetupDI.Register())

                .AddTransient<Control, MainWindow>(nameof(MainWindowVM))
                .AddTransient<Control, MainControlView>(nameof(MainControlVM))
                .AddTransient<Control, HtmlViewerUserControl>(nameof(HtmlViewerVM))

                .AddTransient<IMainWindowVMFactory, MainWindowVMFactory>()
                .AddTransient<IHtmlViewerVMFactory, HtmlViewerVMFactory>()
                .AddTransient<ICancelCommandFactory, CancelCommandFactory>()

                .AddTransient<IMainControlVMFactory, MainControlVMFactory>()
                .AddTransient<IMainControlVMChangeListener, MainControlVMChangeListener>()
                .AddSingleton<IMainControlVMInitializer, MainControlVMInitializer>()

                .AddFactory(resolver => App.Current.ApplicationLifetime);
        }
    }
}