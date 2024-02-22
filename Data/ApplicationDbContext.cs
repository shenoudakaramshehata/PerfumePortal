using CRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRM.Data
{
    public class ApplicationUser : IdentityUser
    {

        public string? UserPic { get; set; }
        public string? FullName { get; set; }
        public bool IsActive { get; set; }

    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().Property(e => e.UserPic);
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Admin", NormalizedName = "Admin".ToUpper() });
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Customer", NormalizedName = "Customer".ToUpper() });
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Operator", NormalizedName = "Operator".ToUpper() });
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Store", NormalizedName = "Store".ToUpper() });
        }
    }
}