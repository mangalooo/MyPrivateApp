
using AutoMapper;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Contact.Classes
{
    public class SharesMappingProfileClass : Profile
    {
        public SharesMappingProfileClass()
        {
            // DepositMoney
            CreateMap<SharesDepositMoneyViewModel, SharesDepositMoney>();
            CreateMap<SharesDepositMoney, SharesDepositMoneyViewModel>();

            // Dividend
            CreateMap<SharesDividendViewModel, SharesDividend>();
            CreateMap<SharesDividend, SharesDividendViewModel>();

            // ErrorHandling
            CreateMap<SharesErrorHandlingViewModel, SharesErrorHandlings>();
            CreateMap<SharesErrorHandlings, SharesErrorHandlingViewModel>();

            // Fee
            CreateMap<SharesFeeViewModel, SharesFee>();
            CreateMap<SharesFee, SharesFeeViewModel>();

            // ImportsFile
            CreateMap<SharesImportsFileViewModel, SharesImportsFile>();
            CreateMap<SharesImportsFile, SharesImportsFileViewModel>();

            // InterestRates
            CreateMap<SharesInterestRatesViewModel, SharesInterestRates>();
            CreateMap<SharesInterestRates, SharesInterestRatesViewModel>();

            // OtherShareImport
            CreateMap<SharesOtherShareImportViewModel, SharesOtherImports>();
            CreateMap<SharesOtherImports, SharesOtherShareImportViewModel>();

            // ProfitOrLoss
            CreateMap<SharesProfitOrLossYearViewModel, SharesProfitOrLossYears>();
            CreateMap<SharesProfitOrLossYears, SharesProfitOrLossYearViewModel>();

            // PurchasedFund
            CreateMap<SharesPurchasedFundViewModel, SharesPurchasedFunds>();
            CreateMap<SharesPurchasedFunds, SharesPurchasedFundViewModel>();

            // SoldFund
            CreateMap<SharesSoldFundViewModel, SharesSoldFunds>();
            CreateMap<SharesSoldFunds, SharesSoldFundViewModel>();

            // Purchased
            CreateMap<SharesPurchasedViewModel, SharesPurchaseds>();
            CreateMap<SharesPurchaseds, SharesPurchasedViewModel>();

            // Sold
            CreateMap<SharesSoldViewModel, SharesSolds>();
            CreateMap<SharesSolds, SharesSoldViewModel>();
        }
    }
}