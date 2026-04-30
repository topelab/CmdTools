using Avalonia.Controls;

namespace AvaloniaProjectRelations.Browser;

public partial class HtmlViewerUserControl : UserControl
{
    private HtmlViewerVM oldVM;

    public HtmlViewerUserControl()
    {
        InitializeComponent();
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
            WebViewPanel.NavigateToString(vm.Content ?? "");
            oldVM?.PropertyChanged -= Vm_PropertyChanged;
            vm.PropertyChanged += Vm_PropertyChanged;
            oldVM = vm;
        }
    }

    private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is HtmlViewerVM vm && e.PropertyName == nameof(HtmlViewerVM.Content))
        {
            WebViewPanel.NavigateToString(vm.Content ?? "");
        }
    }
}