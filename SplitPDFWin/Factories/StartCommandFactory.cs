using ReactiveUI;
using SplitPDFWin.ViewModels;
using System;
using System.IO;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Writer;
using UglyToad.PdfPig;

namespace SplitPDFWin.Factories
{
    internal class StartCommandFactory : CommandFactory<MainWindowViewModel>, IStartCommandFactory
    {
        public override IObservable<bool> CanExecuteObservable
            => context.WhenAnyValue(
                @this => @this.PdfInput,
                @this => @this.PdfOutput,
                (input, output) => !string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(output) && Path.GetExtension(input).Equals(".pdf", StringComparison.CurrentCultureIgnoreCase));

        public override void Execute()
        {
            using PdfDocument document = PdfDocument.Open(context.PdfInput);

            foreach (Page page in document.GetPages())
            {
                using PdfDocumentBuilder builder = new PdfDocumentBuilder();
                builder.AddPage(document, page.Number);
                var pdfBytes = builder.Build();
                File.WriteAllBytes(@$"{context.PdfOutput}\page-{page.Number}.pdf", pdfBytes);
            }
        }
    }
}
