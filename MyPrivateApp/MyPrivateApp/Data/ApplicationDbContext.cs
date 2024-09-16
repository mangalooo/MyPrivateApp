
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Data.Models;
using MyPrivateApp.Data.Models.Farming;
using MyPrivateApp.Data.Models.Games.ManagerZone;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // Private
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Trips> Trips { get; set; }
        public DbSet<FrozenFoods> FrozenFoods { get; set; }
        public DbSet<ShopingList> ShopingLists { get; set; }
        public DbSet<FarmingsActive> FarmingsActive { get; set; }
        public DbSet<FarmingsInactive> FarmingsInactive { get; set; }
        public DbSet<FarmWorks> FarmWorks { get; set; }

        //Hunting
        public DbSet<HuntingMyList> HuntingMyList { get; set; }
        public DbSet<HuntingTeamMembers> HuntingTeamMembers { get; set; }
        public DbSet<HuntingTowerInspection> HuntingTowerInspections { get; set; }

        //Games
        public DbSet<ManagerZoneSoldPlayers> ManagerZoneSoldPlayers { get; set; }
        public DbSet<ManagerZonePurchasedPlayers> ManagerZonePurchasedPlayers { get; set; }

        // Shares
        public DbSet<SharesPurchaseds> SharesPurchaseds { get; set; }
        public DbSet<SharesSolds> SharesSolds { get; set; }
        public DbSet<SharesPurchasedFunds> SharesPurchasedFunds { get; set; }
        public DbSet<SharesSoldFunds> SharesSoldFunds { get; set; }
        public DbSet<SharesFee> SharesFees { get; set; }
        public DbSet<SharesDividend> SharesDividends { get; set; }
        public DbSet<SharesInterestRates> SharesInterestRates { get; set; }
        public DbSet<SharesOtherImports> SharesOtherImports { get; set; }
        public DbSet<SharesDepositMoney> SharesDepositMoney { get; set; }
        public DbSet<SharesTotalAmounts> SharesTotalAmounts { get; set; }
        public DbSet<SharesImportsFile> SharesImportsFiles { get; set; }
        public DbSet<SharesErrorHandlings> SharesErrorHandlings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "User", NormalizedName = "USER", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
            //builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
        }
    }
}