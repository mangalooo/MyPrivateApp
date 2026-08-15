using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Enum;
using MyPrivateApp.Data;
using QuestPDF.Fluent;

namespace MyPrivateApp.Components.FarmWork.Pdf;

public interface IFarmWorksPdfService
{
    Task<byte[]> CreateFarmWorksReportAsync(
        DateOnly from,
        DateOnly to,
        FarmWorkPlaces? place = null,
        string? propertyDesignation = null,
        CancellationToken cancellationToken = default);
}

public sealed class FarmWorksPdfService(IDbContextFactory<ApplicationDbContext> dbFactory) : IFarmWorksPdfService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));

    public async Task<byte[]> CreateFarmWorksReportAsync(
        DateOnly from,
        DateOnly to,
        FarmWorkPlaces? place = null,
        string? propertyDesignation = null,
        CancellationToken cancellationToken = default)
    {
        if (from > to)
            throw new ArgumentException("'from' must be <= 'to'.");

        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken)
            ?? throw new Exception("CreateFarmWorksReportAsync: db == null!");

        string? normalizedPropertyDesignation = string.IsNullOrWhiteSpace(propertyDesignation)
            ? null
            : propertyDesignation.Trim();

        var items = await db.FarmWorks
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var filtered = items
            .Where(x =>
                DateOnly.TryParse(x.Date, out var d) &&
                d >= from &&
                d <= to &&
                (!place.HasValue || x.Place == place.Value) &&
                (normalizedPropertyDesignation is null ||
                 string.Equals(
                     (x.PropertyDesignation ?? string.Empty).Trim(),
                     normalizedPropertyDesignation,
                     StringComparison.OrdinalIgnoreCase)))
            .OrderBy(x => x.Date)
            .ToList();

        var document = new FarmWorksPdfDocument(filtered, from, to);
        return document.GeneratePdf();
    }
}