using Microsoft.EntityFrameworkCore;
using CMCS.Models;

namespace CMCS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<ClaimItem> ClaimItems { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.User)
                .WithMany(u => u.Claims)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClaimItem>()
                .HasOne(ci => ci.Claim)
                .WithMany(c => c.ClaimItems)
                .HasForeignKey(ci => ci.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed initial data
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Email = "lecturer@university.edu",
                    PasswordHash = "lecturer123",
                    Role = RoleType.Lecturer,
                    FirstName = "John",
                    LastName = "Smith",
                    HourlyRate = 350.00m,
                    IsActive = true
                },
                new User
                {
                    UserId = 2,
                    Email = "coordinator@university.edu",
                    PasswordHash = "coordinator123",
                    Role = RoleType.Coordinator,
                    FirstName = "Jane",
                    LastName = "Doe",
                    HourlyRate = 0m,
                    IsActive = true
                },
                new User
                {
                    UserId = 3,
                    Email = "manager@university.edu",
                    PasswordHash = "manager123",
                    Role = RoleType.Manager,
                    FirstName = "Bob",
                    LastName = "Johnson",
                    HourlyRate = 0m,
                    IsActive = true
                },
                new User
                {
                    UserId = 4,
                    Email = "hr@university.edu",
                    PasswordHash = "hr123",
                    Role = RoleType.HR,
                    FirstName = "Sarah",
                    LastName = "Williams",
                    HourlyRate = 0m,
                    IsActive = true
                }
            );
        }
    }
}