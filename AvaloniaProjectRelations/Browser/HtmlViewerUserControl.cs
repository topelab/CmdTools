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
    }

    private void HtmlViewerUserControl_DataContextChanged(object sender, EventArgs e)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (DataContext is HtmlViewerVM vm)
        {
            webViewPanel.NavigateToString(vm.Content ?? "");
            oldVM?.PropertyChanged -= Vm_PropertyChanged;
            vm.PropertyChanged += Vm_PropertyChanged;
            oldVM = vm;
        }
    }

    private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is HtmlViewerVM vm && e.PropertyName == nameof(HtmlViewerVM.Content))
        {
            webViewPanel.NavigateToString(vm.Content ?? "");
        }
    }
}