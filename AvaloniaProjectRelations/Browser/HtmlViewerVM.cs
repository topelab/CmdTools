using Topelab.Core.Avalonia.Base;
using ReactiveUI;

namespace AvaloniaProjectRelations.Browser
{
    internal class HtmlViewerVM : BaseVM
    {
        private Uri uri;
        private string content = "<html><body><p>Hello!!</p></body></html>";

        public Uri Uri { get => uri; set => this.RaiseAndSetIfChanged(ref uri, value); }
        public string Content { get => content; set => this.RaiseAndSetIfChanged(ref content, value); }
    }
}
