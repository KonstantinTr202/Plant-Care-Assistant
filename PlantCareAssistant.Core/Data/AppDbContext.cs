using Microsoft.EntityFrameworkCore;
using PlantCareAssistant.Core.Models;

namespace PlantCareAssistant.Core.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Plant> Plants { get; set; }
        public DbSet<CareRecord> CareRecords { get; set; }

        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=plantcare.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Plant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.CareType).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Lighting).IsRequired();
            });

            modelBuilder.Entity<CareRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.HasOne(e => e.Plant)
                      .WithMany()
                      .HasForeignKey(e => e.PlantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}