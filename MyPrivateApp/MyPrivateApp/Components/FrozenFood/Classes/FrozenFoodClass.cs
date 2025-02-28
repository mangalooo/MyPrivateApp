
using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;
using Hangfire;
using MyPrivateApp.Components.Email.Classes;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.FrozenFood.Classes
{
    public class FrozenFoodClass(ApplicationDbContext db, ILogger<FrozenFoodClass> logger, IMapper mapper) : IFrozenFoodClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<FrozenFoodClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<FrozenFoods?> Get(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.FrozenFoods.FirstOrDefaultAsync(r => r.FrozenFoodsId == id)
                   ?? throw new Exception("Frysvaran hittades inte i databasen!");
        }

        public async Task<string> Add(FrozenFoodViewModel vm)
        {
            if (vm == null || _db == null) return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Name)) return "Ingen datum eller namn ifyllt!";

            try
            {
                FrozenFoods model = ChangeFromViewModelToModel(vm);
                await _db.FrozenFoods.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till en ny frysvara!");
                return $"Gick inte att lägg till en ny frysvara. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(FrozenFoodViewModel vm)
        {
            if (vm == null || vm.FrozenFoodId <= 0 && _db == null) return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Name)) return "Ingen datum eller namn ifyllt!";

            try
            {
                FrozenFoods? getDbModel = await Get(vm.FrozenFoodId);

                if (getDbModel != null) return "Hittar inte frysvra i databasen!";

                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra frysvaran!");
                return $"Gick inte att ändra frysvaran. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(FrozenFoodViewModel vm)
        {
            if (vm == null || vm.FrozenFoodId <= 0 && _db == null) return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            try
            {
                FrozenFoods model = ChangeFromViewModelToModel(vm);
                _db.ChangeTracker.Clear();
                _db.FrozenFoods.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort frysvaran!");
                return $"Gick inte att ta bort frysvaran. Felmeddelande: {ex.Message}";
            }
        }

        public FrozenFoodViewModel ChangeFromModelToViewModel(FrozenFoods model) => _mapper.Map<FrozenFoodViewModel>(model);

        private FrozenFoods ChangeFromViewModelToModel(FrozenFoodViewModel vm) => _mapper.Map<FrozenFoods>(vm);

        public double HowLongTimeInFreezer(DateTime date)
        {
            TimeSpan timeInFreezer = DateTime.UtcNow - date;
            double yearsInFreezer = timeInFreezer.TotalDays / 365;
            return Math.Round(yearsInFreezer, 2, MidpointRounding.AwayFromZero);
        }

        private static void SendEmailTo(IEmailSender emailSender, string getName, FrozenFoods item, string mailFreezer)
        {
            if (emailSender == null) return;

            string subject = $"{getName.ToUpper()} {item.Type} {item.Name}";
            string body = $"Datum: {item.Date} \r\nPlats: {item.Place} \r\nFack: {item.FreezerCompartment} \r\nAntal: {item.Number}";

            ScheduleEmail(emailSender, "Utgående frysvara", mailFreezer, subject, body, mailFreezer);
        }

        private static void ScheduleEmail(IEmailSender emailSender, string title, string from, string subject, string body, string to)
        {
            BackgroundJob.Schedule(() => emailSender.SendEmailFreezer(title, from, subject, body, to), DateTime.Now);
        }

        public void GetOutgoingFrosenFood()
        {
            if (!_db.FrozenFoods.Any()) return;

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
            EmailSender emailSender = new(config);

            string? mailFreezer = config.GetSection("AppSettings")["mailFreezer"];

            if (string.IsNullOrEmpty(mailFreezer)) return;

            var freezerTimes = new Dictionary<string, double>
            {
                { "hare", 6 },
                { "ko", 6 },
                { "rådjur", 6 },
                { "vildsvin", 4 },
                { "älg", 6 },
                { "övrigt", 2 }
            };

            foreach (var item in _db.FrozenFoods)
            {
                if (DateTime.TryParse(item.Date, out DateTime date))
                {
                    string getName = System.Enum.GetName(item.FrozenGoods)?.ToLower() ?? string.Empty;

                    if (freezerTimes.TryGetValue(getName, out double requiredTimeInFreezer) && HowLongTimeInFreezer(date) == requiredTimeInFreezer)
                        SendEmailTo(emailSender, getName, item, mailFreezer);
                }
            }
        }
    }
}