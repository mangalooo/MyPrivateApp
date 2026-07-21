using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

public static class PdfGenerator
{
    public static byte[] CreateSamplePdf()
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);

                page.Content()
                    .Column(col =>
                    {
                        col.Item().Text("Hello from .NET 10 PDF").FontSize(20).Bold();
                        col.Item().Text($"Created: {DateTimeOffset.Now}");
                    });
            });
        }).GeneratePdf();
    }
}