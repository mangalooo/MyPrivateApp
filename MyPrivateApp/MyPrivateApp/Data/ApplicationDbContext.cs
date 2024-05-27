using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Data.Models;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Contacts> Contacts { get; set; } // Kontakter
        public DbSet<Trips> Trips { get; set; } // Resor
        public DbSet<FrozenFoods> FrozenFoods { get; set; } // Resor
        public DbSet<Huntings> Huntings { get; set; } // Resor
        public DbSet<ShopingList> ShopingLists { get; set; } // Resor

        // Aktier
        public DbSet<SharesPurchaseds> SharesPurchaseds { get; set; } // Köpta aktier
        public DbSet<SharesSolds> SharesSolds { get; set; } // Sålda aktier
        public DbSet<SharesFee> SharesFees { get; set; } // Avgifter
        public DbSet<SharesDividend> SharesDividends { get; set; } // Utdelning
        public DbSet<SharesInterestRates> SharesInterestRates { get; set; } // Räntor
        public DbSet<SharesOtherImports> SharesOtherImports { get; set; } // Andra importer
        public DbSet<SharesDepositMoney> SharesDepositMoney { get; set; } // Insättning och uttag av min pengar
        public DbSet<SharesTotalAmounts> SharesTotalAmounts { get; set; } // Totalt belopp insättning av min pengar
        public DbSet<SharesImportsFile> SharesImportsFiles { get; set; } // Information om importerade filer

        // Felhantering
        public DbSet<SharesErrorHandlings> SharesErrorHandlings { get; set; } // Felhantering aktier

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "User", NormalizedName = "USER", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
            //builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
        }
    }
}