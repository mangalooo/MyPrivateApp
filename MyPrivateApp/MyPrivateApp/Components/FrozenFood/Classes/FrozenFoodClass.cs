
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models;
using Hangfire;
using MyPrivateApp.Components.Email.Classes;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.ViewModels;

namespace MyPrivateApp.Components.FrozenFood.Classes
{
    public class FrozenFoodClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<FrozenFoodClass> logger) : IFrozenFoodClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<FrozenFoodClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(FrozenFoodViewModel vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Name))
                return "Ingen datum eller namn ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                FrozenFoods model = ChangeFromViewModelToModel(vm);

                await db.FrozenFoods.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

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
            if (vm == null || vm.FrozenFoodsId <= 0)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Name))
                return "Ingen datum eller namn ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                // Fetch the entity in the same context to ensure tracking
                FrozenFoods? model = await db.FrozenFoods.FirstOrDefaultAsync(r => r.FrozenFoodsId == vm.FrozenFoodsId);
                if (model == null)
                    return "Hittar inte frysvaran i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra frysvaran!");
                return $"Gick inte att ändra frysvaran. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(FrozenFoods model)
        {
            if (model == null || model.FrozenFoodsId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.FrozenFoods.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort frysvaran!");
                return $"Gick inte att ta bort frysvaran. Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public FrozenFoodViewModel ChangeFromModelToViewModel(FrozenFoods model)
        {
            FrozenFoodViewModel vm = new()
            {
                FrozenFoodsId = model.FrozenFoodsId,
                Date = ParseDate(model.Date ?? string.Empty),
                Name = model.Name,
                Type = model.Type,
                Number = model.Number,
                Place = model.Place,
                FreezerCompartment = model.FreezerCompartment,
                FrozenGoods = model.FrozenGoods,
                Weight = model.Weight,
                Notes = model.Notes
            };

            return vm;
        }

        public FrozenFoods ChangeFromViewModelToModel(FrozenFoodViewModel vm)
        {
            FrozenFoods model = new()
            {
                FrozenFoodsId = vm.FrozenFoodsId,
                Date = vm.Date != DateTime.MinValue ? vm.Date.ToString("yyyy-MM-dd") : string.Empty,
                Name = vm.Name,
                Type = vm.Type,
                Number = vm.Number,
                Place = vm.Place,
                FreezerCompartment = vm.FreezerCompartment,
                FrozenGoods = vm.FrozenGoods,
                Weight = vm.Weight,
                Notes = vm.Notes
            };

            return model;
        }

        private static void EditModel(FrozenFoods model, FrozenFoodViewModel vm)
        {
            model.FrozenFoodsId = vm.FrozenFoodsId;
            model.Date = vm.Date != DateTime.MinValue ? vm.Date.ToString("yyyy-MM-dd") : string.Empty;
            model.Name = vm.Name;
            model.Type = vm.Type;
            model.Number = vm.Number;
            model.Place = vm.Place;
            model.FreezerCompartment = vm.FreezerCompartment;
            model.FrozenGoods = vm.FrozenGoods;
            model.Weight = vm.Weight;
            model.Notes = vm.Notes;
        }

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

        public async Task GetOutgoingFrosenFood()
        {
            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("GetOutgoingFrosenFood: db == null!");

                if (!db.FrozenFoods.Any()) 
                    return;

                IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
                EmailSender emailSender = new(config);

                string? mailFreezer = config.GetSection("AppSettings")["mailFreezer"];

                if (string.IsNullOrEmpty(mailFreezer)) 
                    return;

                Dictionary<string, double> freezerTimes = new()
                {
                    { "hare", 6 },
                    { "ko", 6 },
                    { "rådjur", 6 },
                    { "vildsvin", 4 },
                    { "älg", 6 },
                    { "övrigt", 2 }
                };

                foreach (FrozenFoods item in db.FrozenFoods)
                {
                    if (DateTime.TryParse(item.Date, out DateTime date))
                    {
                        string getName = System.Enum.GetName(item.FrozenGoods)?.ToLower() ?? string.Empty;

                        if (freezerTimes.TryGetValue(getName, out double requiredTimeInFreezer) && HowLongTimeInFreezer(date) == requiredTimeInFreezer)
                            SendEmailTo(emailSender, getName, item, mailFreezer);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Frysvara: GetOutgoingFrosenFood felmeddelande!");
            }
        }
    }
}