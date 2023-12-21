using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {}

        public DbSet<Contacts> Contacts { get; set; } // Kontakter
        public DbSet<Trips> Trips { get; set; } // Resor
    }
}