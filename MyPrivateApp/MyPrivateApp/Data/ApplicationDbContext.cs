using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Contacts> Contacts { get; set; } // Kontakter
        public DbSet<Trips> Trips { get; set; } // Resor
        public DbSet<FrozenFoods> FrozenFoods { get; set; } // Resor
        public DbSet<Huntings> Huntings { get; set; } // Resor

        // Aktier
        public DbSet<SharesPurchased> SharesPurchased { get; set; } // Köpta aktier
    }
}