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

        // Aktier
        public DbSet<SharesPurchaseds> SharesPurchaseds { get; set; } // Köpta aktier
        public DbSet<SharesSolds> SharesSolds { get; set; } // Sålda aktier
        public DbSet<SharesFee> SharesFees { get; set; } // Avgifter
        public DbSet<SharesDividend> SharesDividends { get; set; } // Avgifter

        // Felhantering
        public DbSet<SharesErrorHandlings> SharesErrorHandlings { get; set; } // Felhantering aktier
    }
}