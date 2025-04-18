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
        public DbSet<AuditLog> AuditLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    UserSpecificId = "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
                    Name = "Admin",
                    Surname = "Admin",
                    HashedPassword = "$2a$11$O7apxg/GcxbEflsqKA4YFuk2RHCOsvFcuSI/Rz6ud7D/8FUdvB51C",
                    Email = "admin@admin.com",
                    Role = "Admin",
                    Status = "Active",
                    CreatedAt = new DateTime(2025, 1, 1),
                    ChangedAt = new DateTime(2025, 1, 1),
                    VersionNum = 1,
                    isDeleted = 0
                }
            );
        }
    }
}
