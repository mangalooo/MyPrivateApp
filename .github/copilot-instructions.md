using MyPrivateApp.Components.Enum;
using MyPrivateApp.Data.Models.Hunting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MyPrivateApp.Components.Hunting.Pdf;

public sealed class HuntingTowerInspectionTodoPdfDocument : IDocument
{
    private readonly IReadOnlyList<HuntingTowerInspection> _items;

    public HuntingTowerInspectionTodoPdfDocument(IReadOnlyList<HuntingTowerInspection> items)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
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
}# Copilot Instructions

## General Guidelines
- Prioritize Blazor guidance over MVC/Razor Pages for projects targeting .NET 10, especially when working with Blazor Web Apps that include a WebAssembly project.
- Integrate solution changes into existing Blazor pages rather than providing hard-coded example lists, particularly for FarmWorks/FarmWorksPlanning UI behavior.
- Design compact UI layouts where related list items are displayed on a single row, and limit list items to at most two rows. Remove the 'Nästa lön' badge.
- Ensure that list sections have a visible title and an outer border/frame around the list container.
- When a user requests a change in a specific code snippet, limit the solution to just that part instead of suggesting larger page changes.
- Ensure that text in UI components has a consistent style unless otherwise specified.
- Style the navigation menu with a black-to-green gradient background instead of a solid black background.
- Do not show divider lines in views when there is no information to display, including after the last data row in grid/list views.
- Verify UI issue fixes against actual project CSS/layout files instead of only providing generic suggestions.

## Project-Specific Rules
- Ensure that all instructions and code examples are tailored to the Blazor framework, leveraging its features and best practices.
- For the hunting tower inspection PDF feature, place the download button directly in `HuntingTowerInspectionPage.razor` instead of creating a separate page.
- Implement new date search solutions in FarmWork pages to behave like the existing QuickGrid column filters for 'Plats' and 'Område', rather than as separate external filters.