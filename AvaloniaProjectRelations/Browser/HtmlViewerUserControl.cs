using Avalonia.Controls;

namespace AvaloniaProjectRelations.Browser;

public partial class HtmlViewerUserControl : UserControl
{
    private HtmlViewerVM oldVM;
    private readonly NativeWebView webViewPanel;

    public HtmlViewerUserControl()
    {
        if (Design.IsDesignMode)
        {
            return;
        }
        var scrollViewer = new ScrollViewer();
        webViewPanel = new NativeWebView();
        scrollViewer.Content = webViewPanel;
        Content = scrollViewer;
        DataContextChanged += HtmlViewerUserControl_DataContextChanged;

        webViewPanel.EnvironmentRequested += (s, e) =>
        {
            e.EnableDevTools = true;
        };
    }

    private void HtmlViewerUserControl_DataContextChanged(object sender, EventArgs e)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (DataContext is HtmlViewerVM vm)
        {
            if (vm.Uri == null)
            {
                webViewPanel.NavigateToString(vm.Content ?? "");

            }
            else
            {
                webViewPanel.Navigate(vm.Uri);
            }
            oldVM?.PropertyChanged -= OnHtmlViewerVMPropertyChanged;
            vm.PropertyChanged += OnHtmlViewerVMPropertyChanged;
            oldVM = vm;
        }
    }

    private void OnHtmlViewerVMPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is HtmlViewerVM vm)
        {
            switch (e.PropertyName)
            {
                case nameof(HtmlViewerVM.Content):
                    webViewPanel.NavigateToString(vm.Content ?? "");
                    break;
                case nameof(HtmlViewerVM.Uri):
                    if (vm.Uri != null)
                    {
                        webViewPanel.Navigate(vm.Uri);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}