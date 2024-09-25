﻿
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public class ManagerZoneSoldClass : IManagerZoneSoldClass
    {
        private static ManagerZoneSoldPlayers? Get(ApplicationDbContext db, int? id) => db.ManagerZoneSoldPlayers.Any(r => r.ManagerZoneSoldPlayerId == id) ?
                                                                                            db.ManagerZoneSoldPlayers.FirstOrDefault(r => r.ManagerZoneSoldPlayerId == id) :
                                                                                                throw new Exception("Objektet såld spelare hittades inte i databasen!");

        public string Add(ApplicationDbContext db, ManagerZoneSoldViewModels vm)
        {
            if (vm != null && db != null)
            {
                if (vm.PurchasedDate != DateTime.MinValue && vm.SoldDate != DateTime.MinValue && vm.PurchaseAmount > 0 && vm.SalaryTotal > 0
                    && vm.PurchaseAmount > 0 && vm.TrainingModeTotalCost > 0 && vm.SoldAmount > 0)
                {
                    try
                    {
                        ManagerZoneSoldPlayers model = ChangeFromViewModelToModel(vm);

                        db.ManagerZoneSoldPlayers.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att lägg till en ny spelare. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Datum och kostnader måste fyllas i!";

            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, ManagerZoneSoldViewModels vm)
        {
            if (vm != null && vm.ManagerZoneSoldPlayerId > 0 && db != null)
            {
                if (vm.PurchasedDate != DateTime.MinValue && vm.SoldDate != DateTime.MinValue && vm.PurchaseAmount > 0 && vm.SalaryTotal > 0
                    && vm.PurchaseAmount > 0 && vm.TrainingModeTotalCost > 0 && vm.SoldAmount > 0)
                {
                    try
                    {
                        ManagerZoneSoldPlayers getDbModel = Get(db, vm.ManagerZoneSoldPlayerId);

                        if (getDbModel != null)
                        {
                            getDbModel.ManagerZoneSoldPlayerId = vm.ManagerZoneSoldPlayerId;
                            getDbModel.PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd");
                            getDbModel.SoldDate = vm.SoldDate.ToString("yyyy-MM-dd");
                            getDbModel.PurchaseAmount = vm.PurchaseAmount;
                            getDbModel.SalaryTotal = vm.SalaryTotal;
                            getDbModel.DaysInTheClub = vm.DaysInTheClub;
                            getDbModel.TrainingModeTotalCost = vm.TrainingModeTotalCost;
                            getDbModel.SoldAmount = vm.SoldAmount;
                            getDbModel.MoneyProfitOrLoss = vm.MoneyProfitOrLoss;
                            getDbModel.Note = vm.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte spelaren i databasen!";
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att ändra spelaren. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Datum och kostnader måste fyllas i!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, ManagerZoneSoldViewModels vm)
        {
            if (vm != null && vm.ManagerZoneSoldPlayerId > 0 && db != null)
            {
                try
                {
                    ManagerZoneSoldPlayers model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.ManagerZoneSoldPlayers.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Gick inte att ta bort spelaren. Felmeddelande: {ex.Message}";
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public ManagerZoneSoldViewModels ChangeFromModelToViewModel(ManagerZoneSoldPlayers model)
        {
            DateTime purchasedDate = DateTime.Parse(model.PurchasedDate);
            DateTime soldDate = DateTime.Parse(model.SoldDate);

            ManagerZoneSoldViewModels vm = new()
            {
                ManagerZoneSoldPlayerId = model.ManagerZoneSoldPlayerId,
                PurchasedDate = purchasedDate,
                SoldDate = soldDate,
                DaysInTheClub = model.DaysInTheClub,
                PurchaseAmount = model.PurchaseAmount,
                SalaryTotal = model.SalaryTotal,
                TrainingModeTotalCost = model.TrainingModeTotalCost,
                SoldAmount = model.SoldAmount,
                MoneyProfitOrLoss = model.MoneyProfitOrLoss,
                Note = model.Note
            };

            return vm;
        }

        private static ManagerZoneSoldPlayers ChangeFromViewModelToModel(ManagerZoneSoldViewModels vm)
        {
            ManagerZoneSoldPlayers managerZoneSoldPlayers = new()
            {
                ManagerZoneSoldPlayerId = vm.ManagerZoneSoldPlayerId,
                PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd"),
                SoldDate = vm.SoldDate.ToString("yyyy-MM-dd"),
                DaysInTheClub = vm.DaysInTheClub,
                PurchaseAmount = vm.PurchaseAmount,
                SalaryTotal = vm.SalaryTotal,
                TrainingModeTotalCost = vm.TrainingModeTotalCost,
                SoldAmount = vm.SoldAmount,
                MoneyProfitOrLoss = vm.MoneyProfitOrLoss,
                Note = vm.Note
            };

            return managerZoneSoldPlayers;
        }
    }
}