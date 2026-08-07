using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MyPrivateApp.Data.Models.FarmWork;

namespace MyPrivateApp.Components.FarmWork.Pdf;

public sealed class FarmWorksPdfDocument : IDocument
{
    private readonly IReadOnlyList<FarmWorks> _items;
    private readonly DateOnly _from;
    private readonly DateOnly _to;

    public FarmWorksPdfDocument(IReadOnlyList<FarmWorks> items, DateOnly from, DateOnly to)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
        _from = from;
        _to = to;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        double totalHours = _items.Sum(x => x.Hours);

        container.Page(page =>
        {
            page.Margin(30);
            page.Size(PageSizes.A4);

            page.Header().Column(col =>
            {
                col.Item().Text("Tidsrapportering Magnus Leanderson").FontSize(18).SemiBold();
                col.Item().Text("");
                col.Item().Text($"Period: {_from:yyyy-MM-dd} - {_to:yyyy-MM-dd}").FontSize(12).FontColor(Colors.Grey.Darken2);
                col.Item().LineHorizontal(1);
            });

            page.Content().PaddingTop(10).Column(col =>
            {
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(90);   // Datum
                        columns.ConstantColumn(70);   // Plats
                        columns.ConstantColumn(85);   // Område
                        columns.ConstantColumn(65);   // Timmar
                        columns.RelativeColumn(1);    // Notering
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("Datum");
                        header.Cell().Element(HeaderCell).Text("Plats");
                        header.Cell().Element(HeaderCell).Text("Område");
                        header.Cell().Element(HeaderCell).Text("Timmar");
                        header.Cell().Element(HeaderCell).Text("Notering");

                        static IContainer HeaderCell(IContainer container) => container
                            .DefaultTextStyle(x => x.SemiBold())
                            .PaddingVertical(5)
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten1);
                    });

                    foreach (FarmWorks item in _items)
                    {
                        table.Cell().Element(RowCell).Text(item.Date ?? string.Empty);
                        table.Cell().Element(RowCell).Text(item.Place.ToString());
                        table.Cell().Element(RowCell).Text(item.Area ?? string.Empty);
                        table.Cell().Element(RowCell).Text(item.Hours.ToString("0.##"));
                        table.Cell().Element(RowCell).Text(item.Note ?? string.Empty);
                    }

                    static IContainer RowCell(IContainer container) => container
                        .PaddingVertical(3)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten1);
                });

                col.Item().Text("");
                col.Item().LineHorizontal(1);

                col.Item()
                    .AlignRight()
                    .PaddingTop(10)
                    .Text($"Totalt timmar: {totalHours:0.##}")
                    .SemiBold();
            });

            page.Footer().AlignRight().Text(x =>
            {
                x.Span("Sida ");
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        });
    }
}