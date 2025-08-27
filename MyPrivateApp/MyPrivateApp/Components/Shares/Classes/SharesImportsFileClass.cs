
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;
using MyPrivateApp.Components.Shares.Classes.Interface;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesImportsFileClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesImportsFileClass> logger) : ISharesImportsFileClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesImportsFileClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(SharesImportsFileViewModel vm)
        {
            try
            {
                if (vm == null)
                    return "Hittar ingen data från formuläret!";

                if (vm.Date == DateTime.MinValue)
                    return "Ingen datum ifyllt!";

                SharesImportsFile model = ChangeFromViewModelToModel(vm);

                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                await db.SharesImportsFiles.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Felmeddelande: {ex.Message}.";
            }
        }

        public async Task<string> Edit(SharesImportsFileViewModel vm)
        {
            try
            {
                if (vm == null || vm.SharesImportsFileId <= 0)
                    return "Hittar ingen data från formuläret!";

                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                SharesImportsFile? model = await db.SharesImportsFiles.FirstOrDefaultAsync(r => r.SharesImportsFileId == vm.SharesImportsFileId)
                    ?? throw new Exception("Hittar inte importen i databasen!");

                if (model == null)
                    return "Hittar inte importen i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Felmeddelande: {ex.Message}.";
            }
        }

        public async Task<string> Delete(SharesImportsFile model)
        {
            try
            {
                if (model == null || model.SharesImportsFileId <= 0)
                    return "Hittar ingen data från formuläret!";

                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.ChangeTracker.Clear();
                db.SharesImportsFiles.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Felmeddelande: {ex.Message}.";
            }
        }

        private static void EditModel(SharesImportsFile model, SharesImportsFileViewModel vm)
        {
            model.Date = vm.Date.ToString("yyyy-MM-dd");
            model.FileName = vm.FileName;
            model.NumbersOfTransaction = vm.NumbersOfTransaction;
            model.Errors = vm.Errors;
            model.Note = vm.Note;
        }

        public SharesImportsFileViewModel ChangeFromModelToViewModel(SharesImportsFile model)
        {
            return new SharesImportsFileViewModel
            {
                SharesImportsFileId = model.SharesImportsFileId,
                Date = model.Date != null ? DateTime.Parse(model.Date) : DateTime.MinValue,
                FileName = model.FileName,
                NumbersOfTransaction = model.NumbersOfTransaction,
                Errors = model.Errors,
                Note = model.Note
            };
        }

        private static SharesImportsFile ChangeFromViewModelToModel(SharesImportsFileViewModel vm)
        {
            return new SharesImportsFile
            {
                SharesImportsFileId = vm.SharesImportsFileId,
                Date = vm.Date != DateTime.MinValue ? vm.Date.ToString("yyyy-MM-dd") : null,
                FileName = vm.FileName,
                NumbersOfTransaction = vm.NumbersOfTransaction,
                Errors = vm.Errors,
                Note = vm.Note
            };
        }
    }
}