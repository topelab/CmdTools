using AvaloniaProjectRelations.Commands;

namespace AvaloniaProjectRelations.Browser
{
    internal class HtmlViewerVMFactory : IHtmlViewerVMFactory
    {
        private readonly ICancelCommandFactory cancelCommandFactory;

        public HtmlViewerVMFactory(ICancelCommandFactory cancelCommandFactory)
        {
            this.cancelCommandFactory = cancelCommandFactory;
        }

        public HtmlViewerVM Create(string content)
        {
            HtmlViewerVM htmlViewerVM = new()
            {
                Content = content
            };
            htmlViewerVM.CancelCommand = cancelCommandFactory.Create(htmlViewerVM);
            return htmlViewerVM;
        }
    }
}
