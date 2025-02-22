
using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;
using Hangfire;
using MyPrivateApp.Components.Email.Classes;
using MyPrivateApp.Components.Enum;

namespace MyPrivateApp.Components.FrozenFood.Classes
{
    public class FrozenFoodClass : IFrozenFoodClass
    {
        private static FrozenFoods? Get(ApplicationDbContext db, int? id) => db.FrozenFoods.Any(r => r.FrozenFoodsId == id) ?
                                                                        db.FrozenFoods.FirstOrDefault(r => r.FrozenFoodsId == id) :
                                                                            throw new Exception("Frysvaran hittades inte i databasen!");

        public string Add(ApplicationDbContext db, FrozenFoodViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Name))
                {
                    try
                    {
                        FrozenFoods model = ChangeFromViewModelToModel(vm);

                        db.FrozenFoods.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att lägg till en ny frysvara. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Ingen datum eller namn ifyllt!";

            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";


            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, FrozenFoodViewModel vm)
        {
            if (vm != null && vm.FrozenFoodId > 0 && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Name))
                {
                    try
                    {
                        FrozenFoods getDbModel = Get(db, vm.FrozenFoodId);

                        if (getDbModel != null)
                        {
                            getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                            getDbModel.Name = vm.Name;
                            getDbModel.Type = vm.Type;
                            getDbModel.Number = vm.Number;
                            getDbModel.Place = vm.Place;
                            getDbModel.FreezerCompartment = vm.FreezerCompartment;
                            getDbModel.FrozenGoods = vm.FrozenGoods;
                            getDbModel.Weight = vm.Weight;
                            getDbModel.Notes = vm.Notes;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte aktien i databasen!";
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att ändra frysvaran. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Ingen datum eller namn ifyllt!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, FrozenFoodViewModel vm, bool import)
        {
            if (vm != null && vm.FrozenFoodId > 0 && db != null)
            {
                try
                {
                    FrozenFoods model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.FrozenFoods.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Gick inte att ta bort frysvaran. Felmeddelande: {ex.Message}";
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public FrozenFoodViewModel ChangeFromModelToViewModel(FrozenFoods model)
        {
            DateTime date = DateTime.Parse(model.Date);

            FrozenFoodViewModel fee = new()
            {
                FrozenFoodId = model.FrozenFoodsId,
                Name = model.Name,
                Type = model.Type,
                Date = date,
                Number = model.Number,
                Place = model.Place,
                FreezerCompartment = model.FreezerCompartment,
                FrozenGoods = model.FrozenGoods,
                Weight = model.Weight,
                Notes = model.Notes
            };

            return fee;
        }

        private static FrozenFoods ChangeFromViewModelToModel(FrozenFoodViewModel vm)
        {
            FrozenFoods frozenFood = new()
            {
                FrozenFoodsId = vm.FrozenFoodId,
                Name = vm.Name,
                Type = vm.Type,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                Number = vm.Number,
                Place = vm.Place,
                FreezerCompartment = vm.FreezerCompartment,
                FrozenGoods = vm.FrozenGoods,
                Weight = vm.Weight,
                Notes = vm.Notes
            };

            return frozenFood;
        }

        public double HowLongTimeInFreezer(DateTime date)
        {
            double days = (DateTime.Now - date).Days;
            double result = days / 365;

            return double.Round(result, 2, MidpointRounding.AwayFromZero);
        }

        private static void SendEmailTo(EmailSender emailSender, string getName, FrozenFoods item, string mailFreezer)
        {
            if (emailSender == null) return;

            BackgroundJob.Schedule(() => emailSender.SendEmailFreezer(
                                "Utgående frysvara", 
                                mailFreezer, 
                                $"{getName.ToUpper()} {item.Type} {item.Name}",
                                $"Datum: {item.Date} \r\nPlats: {item.Place} \r\nFack: {item.FreezerCompartment} \r\nAntal: {item.Number}",
                                mailFreezer),
                                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
        }

        public void GetOutgoingFrosenFood(ApplicationDbContext db)
        {
            if (!db.FrozenFoods.Any()) return;

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
            EmailSender emailSender = new(config);

            string? mailFreezer = config.GetSection("AppSettings")["mailFreezer"];

            if (string.IsNullOrEmpty(mailFreezer)) return;

            foreach (FrozenFoods item in db.FrozenFoods)
            {
                DateTime date = Convert.ToDateTime(item.Date);
                string getName = string.Empty;

                foreach (int frozenGoods in Enum.FreezerFrozenGoods.GetValues(typeof(FreezerFrozenGoods)))
                {
                    Type enumType = typeof(FreezerFrozenGoods);
                    getName = Enum.FreezerFrozenGoods.GetName(enumType, frozenGoods).ToLower();

                    if (frozenGoods == (int)item.FrozenGoods)
                        break;
                }

                switch (getName)
                {
                    case "hare":
                        if (HowLongTimeInFreezer(date) == 6)
                            SendEmailTo(emailSender, getName, item, mailFreezer);
                        break;

                    case "ko":
                        if (HowLongTimeInFreezer(date) == 6)
                            SendEmailTo(emailSender, getName, item, mailFreezer);
                        break;

                    case "rådjur":
                        if (HowLongTimeInFreezer(date) == 6)
                            SendEmailTo(emailSender, getName, item, mailFreezer);
                        break;

                    case "vildsvin":
                        if (HowLongTimeInFreezer(date) == 4)
                            SendEmailTo(emailSender, getName, item, mailFreezer);
                        break;

                    case "älg":
                        if (HowLongTimeInFreezer(date) == 6)
                            SendEmailTo(emailSender, getName, item, mailFreezer);
                        break;

                    case "övrigt":
                        if (HowLongTimeInFreezer(date) == 2)
                            SendEmailTo(emailSender, getName, item, mailFreezer);
                        break;
                }
            }
        }
    }
}