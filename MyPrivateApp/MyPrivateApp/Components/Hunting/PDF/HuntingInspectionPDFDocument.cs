using MyPrivateApp.Components.Enum;
using MyPrivateApp.Data.Models.Hunting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MyPrivateApp.Components.Hunting.Pdf;

public sealed class HuntingTowerInspectionTodoPdfDocument : IDocument
{
    private readonly IReadOnlyList<HuntingTowerInspection> _items;
    private readonly HuntingPlaces? _place;

    public HuntingTowerInspectionTodoPdfDocument(IReadOnlyList<HuntingTowerInspection> items, HuntingPlaces? place)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
        _place = place;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.Size(PageSizes.A4);

            page.Header().Column(col =>
            {
                col.Item().Text("Jakttornsinspektioner - Att göra").FontSize(18).SemiBold();
                col.Item().Text($"Skapad: {DateTime.Now:yyyy-MM-dd}")
                    .FontSize(12)
                    .FontColor(Colors.Grey.Darken2);

                col.Item().Text($"Plats: {(_place.HasValue ? _place.Value.ToString() : "Skog + Karlabo")}")
                    .FontSize(12)
                    .FontColor(Colors.Grey.Darken2);

                col.Item().Text($"Antal poster: {_items.Count}")
                    .FontSize(12)
                    .FontColor(Colors.Grey.Darken2);

                col.Item().LineHorizontal(1);
            });

            page.Content().PaddingTop(10).Column(col =>
            {
                if (_items.Count == 0)
                {
                    col.Item().Text("Det finns inga jakttornsinspektioner markerade som Att göra.");
                    return;
                }

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(90);
                        columns.ConstantColumn(90);
                        columns.ConstantColumn(55);
                        columns.ConstantColumn(80);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("Besiktad");
                        header.Cell().Element(HeaderCell).Text("Plats");
                        header.Cell().Element(HeaderCell).Text("Nr");
                        header.Cell().Element(HeaderCell).Text("Att göra");
                        header.Cell().Element(HeaderCell).Text("Anteckningar");

                        static IContainer HeaderCell(IContainer container) => container
                            .DefaultTextStyle(x => x.SemiBold())
                            .PaddingVertical(5)
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten1);
                    });

                    for (int i = 0; i < _items.Count; i++)
                    {
                        HuntingTowerInspection item = _items[i];
                        bool isLastRow = i == _items.Count - 1;

                        table.Cell().Element(c => RowCell(c, isLastRow)).Text(item.LastInspected ?? string.Empty);
                        table.Cell().Element(c => RowCell(c, isLastRow)).Text(item.Place.ToString());
                        table.Cell().Element(c => RowCell(c, isLastRow)).Text(item.Number ?? string.Empty);
                        table.Cell().Element(c => RowCell(c, isLastRow)).Text(item.Todo == HuntingTodo.Inget ? string.Empty : item.Todo.ToString());
                        table.Cell().Element(c => RowCell(c, isLastRow)).Text(item.Note ?? string.Empty);
                    }

                    static IContainer RowCell(IContainer container, bool isLastRow)
                    {
                        var result = container.PaddingVertical(3);

                        if (!isLastRow)
                            result = result.BorderBottom(1).BorderColor(Colors.Grey.Lighten1);

                        return result;
                    }
                });
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