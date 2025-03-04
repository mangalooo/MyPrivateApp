
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public interface IMZPurchasedClass
    {
        Task<string> Add(MZPurchasedPlayersViewModels vm);
        Task<string> Edit(MZPurchasedPlayersViewModels vm);
        Task<string> Sell(MZPurchasedPlayersViewModels vm);
        Task<string> Delete(MZPurchasedPlayersViewModels vm);
        int DaysInTheClub(DateTime PurchasedDate);
        int TotalSalary(DateTime PurchasedDate, int salary);
        double TotalCost(DateTime PurchasedDate, int salary, int PurchaseAmount, int TrainingModeTotalCost, double SaleCharge);
        MZPurchasedPlayersViewModels ChangeFromModelToViewModel(MZPurchasedPlayers model);
    }
}
