using Topelab.Core.Avalonia.Base;
using ReactiveUI;

namespace AvaloniaProjectRelations.Browser
{
    internal class HtmlViewerVM : BaseVM
    {
        private string url;
        private string content = "<html><body><p>Hello!!</p></body></html>";

        public string Url { get => url; set => this.RaiseAndSetIfChanged(ref url, value); }
        public string Content { get => content; set => this.RaiseAndSetIfChanged(ref content, value); }
    }
}
