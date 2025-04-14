using ImelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ImelAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin",
                    Surname = "Admin",
                    HashedPassword = "$2a$11$O7apxg/GcxbEflsqKA4YFuk2RHCOsvFcuSI/Rz6ud7D/8FUdvB51C",
                    Email = "admin@admin.com",
                    Role = "Admin",
                    Status = "Active",
                    CreatedAt = new DateTime(2025, 1, 1),
                    VersionNum = 1,
                    isDeleted = 0
                }
            );
        }
    }
}
