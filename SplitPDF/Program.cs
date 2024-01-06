using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Writer;

using PdfDocument document = PdfDocument.Open(@"C:\Users\detos\Downloads\test.pdf");

foreach (Page page in document.GetPages())
{
    using PdfDocumentBuilder builder = new PdfDocumentBuilder();
    builder.AddPage(document, page.Number);
    var pdfBytes = builder.Build();
    File.WriteAllBytes(@$"C:\Users\detos\Downloads\test-{page.Number}.pdf", pdfBytes);
}
