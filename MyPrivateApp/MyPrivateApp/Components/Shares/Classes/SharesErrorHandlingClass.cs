
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesErrorHandlingClass(ApplicationDbContext db, ILogger<SharesDepositMoneyClass> logger, IMapper mapper)
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<SharesDepositMoneyClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<SharesErrorHandlings?> Get(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await _db.SharesErrorHandlings.FirstOrDefaultAsync(r => r.ErrorHandlingsId == id)
                ?? throw new Exception("Falhanteringen hittades inte i databasen!");
        }

        public async Task Edit(ApplicationDbContext db, SharesErrorHandlingViewModel vm)
        {
            if (vm != null)
            {
                SharesErrorHandlings? getDbModel = await Get(vm.ErrorHandlingsId);

                try
                {
                    if (getDbModel != null)
                    {
                        _mapper.Map(vm, getDbModel);
                        db.SaveChanges();
                        await _db.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    string message = $"Ändra felhandtering: \r\nDatum: {vm.Date} \r\nAnteckningar: {vm.Note} \r\nFel hantering: {vm.ErrorMessage}!";
                    _logger.LogError(ex, message);
                }
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public SharesErrorHandlings ChangeFromViewModelToModel(SharesErrorHandlingViewModel vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            SharesErrorHandlings model = _mapper.Map<SharesErrorHandlings>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            return model;
        }

        public SharesErrorHandlingViewModel ChangeFromModelToViewModel(SharesErrorHandlings model)
        {
            ArgumentNullException.ThrowIfNull(model);

            SharesErrorHandlingViewModel vm = _mapper.Map<SharesErrorHandlingViewModel>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            return vm;
        }

        public async Task Delete(SharesErrorHandlingViewModel vm)
        {
            SharesErrorHandlings model = ChangeFromViewModelToModel(vm);

            if (model != null)
            {
                try
                {
                    _db.ChangeTracker.Clear();
                    _db.Remove(model);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    string message = $"Ta bort felhandtering: \r\nDatum: {vm.Date} \r\nAnteckningar: {vm.Note} \r\nFel hantering: {vm.ErrorMessage}!";
                    _logger.LogError(ex, message);
                }
            }
        }
    }
}