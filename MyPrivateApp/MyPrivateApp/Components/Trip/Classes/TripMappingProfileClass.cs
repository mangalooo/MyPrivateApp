
using AutoMapper;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.Trip.Classes
{
    public class TripMappingProfileClass : Profile
    {
        public TripMappingProfileClass()
        {
            CreateMap<TripsViewModel, Trips>();
            CreateMap<Trips, TripsViewModel>();
        }
    }
}