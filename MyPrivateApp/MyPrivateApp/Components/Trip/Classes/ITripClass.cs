﻿
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.Trip.Classes
{
    public interface ITripClass
    {
        Task<string> Add(TripsViewModel vm);
        Task<string> Edit(TripsViewModel vm);
        Task<string> Delete(TripsViewModel vm);
        TripsViewModel ChangeFromModelToViewModel(Trips model);
    }
}