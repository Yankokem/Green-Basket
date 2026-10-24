using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Green_Basket.Models;

namespace Green_Basket.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; } = null!;
        public DbSet<RecoveryProcedure> RecoveryProcedures { get; set; } = null!;
        public DbSet<Contact> Contacts { get; set; } = null!;
        public DbSet<RecoveryTest> RecoveryTests { get; set; } = null!;
        public DbSet<RecoveryTask> RecoveryTasks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Asset -> RecoveryProcedures (Cascade delete)
            builder.Entity<RecoveryProcedure>()
                .HasOne(p => p.Asset)
                .WithMany(a => a.RecoveryProcedures)
                .HasForeignKey(p => p.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Asset -> Contacts (Cascade delete)
            builder.Entity<Contact>()
                .HasOne(c => c.Asset)
                .WithMany(a => a.Contacts)
                .HasForeignKey(c => c.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Asset -> RecoveryTests (Cascade delete)
            builder.Entity<RecoveryTest>()
                .HasOne(t => t.Asset)
                .WithMany(a => a.RecoveryTests)
                .HasForeignKey(t => t.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Asset -> RecoveryTasks (Cascade delete)
            builder.Entity<RecoveryTask>()
                .HasOne(k => k.Asset)
                .WithMany(a => a.RecoveryTasks)
                .HasForeignKey(k => k.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
