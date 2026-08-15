using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Enum;
using MyPrivateApp.Data;
using QuestPDF.Fluent;

namespace MyPrivateApp.Components.Hunting.Pdf;

public interface IHuntingTowerInspectionPdfService
{
    Task<byte[]> CreateTodoReportAsync(HuntingPlaces? place = null, CancellationToken cancellationToken = default);
}

public sealed class HuntingTowerInspectionPdfService(IDbContextFactory<ApplicationDbContext> dbFactory) : IHuntingTowerInspectionPdfService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));

    public async Task<byte[]> CreateTodoReportAsync(HuntingPlaces? place = null, CancellationToken cancellationToken = default)
    {
        await using ApplicationDbContext db = await _dbFactory.CreateDbContextAsync(cancellationToken)
            ?? throw new Exception("CreateTodoReportAsync: db == null!");

        HuntingPlaces? normalizedPlace = place switch
        {
            HuntingPlaces.Skog => HuntingPlaces.Skog,
            HuntingPlaces.Karlabo => HuntingPlaces.Karlabo,
            _ => null
        };

        var items = await db.HuntingTowerInspections
            .AsNoTracking()
            .Where(x => x.InspectedTodo && (!normalizedPlace.HasValue || x.Place == normalizedPlace.Value))
            .OrderBy(x => x.Place)
            .ThenBy(x => x.Number)
            .ToListAsync(cancellationToken);

        var document = new HuntingTowerInspectionTodoPdfDocument(items, normalizedPlace);
        return document.GeneratePdf();
    }
}