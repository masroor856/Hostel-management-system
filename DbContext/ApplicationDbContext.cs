using HostelManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HostelManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==============================
        // DATABASE TABLES
        // ==============================

        public DbSet<Student> Students { get; set; }

        public DbSet<AdminProfile> AdminProfiles { get; set; }

        public DbSet<HostelRoom> HostelRooms { get; set; }

        public DbSet<HostelApplication> HostelApplications { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<RoomAllocation> RoomAllocations { get; set; }

        public DbSet<ContactMessage> ContactMessages { get; set; }


        // ==============================
        // MODEL CONFIGURATION
        // ==============================

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // AdminProfile → IdentityUser
            builder.Entity<AdminProfile>()
                .HasIndex(a => a.IdentityUserId)
                .IsUnique();
        }
    }
}