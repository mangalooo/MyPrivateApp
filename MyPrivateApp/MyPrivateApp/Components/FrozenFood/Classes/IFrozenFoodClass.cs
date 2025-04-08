﻿using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.FrozenFood.Classes
{
    public interface IFrozenFoodClass
    {
        Task<string> Add(FrozenFoodViewModel vm);
        Task<string> Edit(FrozenFoodViewModel vm);
        Task<string> Delete(FrozenFoodViewModel vm);
        FrozenFoodViewModel ChangeFromModelToViewModel(FrozenFoods model);
        double HowLongTimeInFreezer(DateTime date);
        void GetOutgoingFrosenFood();
    }
}
