using ReactiveUI;
using SplitPDFWin.ViewModels;
using System;
using System.IO;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Writer;
using UglyToad.PdfPig;
using System.Diagnostics;

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
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(context.PdfInput);
            using PdfDocument document = PdfDocument.Open(context.PdfInput);

            foreach (Page page in document.GetPages())
            {
                string outputFileName = @$"{context.PdfOutput}\{fileNameWithoutExtension}-{page.Number:0000}.pdf";

                if (context.FileOverride || !File.Exists(outputFileName))
                {
                    using PdfDocumentBuilder builder = new PdfDocumentBuilder();
                    builder.AddPage(document, page.Number);
                    var pdfBytes = builder.Build();
                    File.WriteAllBytes(outputFileName, pdfBytes);
                }
            }

            Process.Start(new ProcessStartInfo(context.PdfOutput) { UseShellExecute = true });
        }
    }
}
