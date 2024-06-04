
using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.Trip.Classes
{
    public interface ITripClass
    {
        string Add(ApplicationDbContext db, TripsViewModel vm, bool import);
        string Edit(ApplicationDbContext db, TripsViewModel vm);
        string Delete(ApplicationDbContext db, TripsViewModel vm, bool import);
        TripsViewModel ChangeFromModelToViewModel(Trips model);
    }
}
