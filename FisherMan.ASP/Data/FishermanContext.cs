using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Data
{
    public class FishermanContext : DbContext
    {
        public FishermanContext(DbContextOptions<FishermanContext> options) : base(options) { }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Vessel> Vessels { get; set; }
        public DbSet<Permit> Permits { get; set; }
        public DbSet<LogbookEntry> LogbookEntries { get; set; }
        public DbSet<FishUnloading> FishUnloadings { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<AmateurTicket> AmateurTickets { get; set; }
        public DbSet<AmateurCatch> AmateurCatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Person
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).HasMaxLength(200);
                entity.Property(e => e.IdentificationNumber).HasMaxLength(50);
            });

            // Vessel
            modelBuilder.Entity<Vessel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InternationalNumber).HasMaxLength(50);
                entity.Property(e => e.Length).HasPrecision(10, 2);
                entity.Property(e => e.Width).HasPrecision(10, 2);
                entity.Property(e => e.Tonnage).HasPrecision(10, 2);
                entity.Property(e => e.Draft).HasPrecision(10, 2);
                entity.Property(e => e.EnginePower).HasPrecision(10, 2);
                entity.Property(e => e.AvgFuelConsumption).HasPrecision(10, 2);
                entity.HasOne(e => e.Owner)
                      .WithMany(p => p.OwnedVessels)
                      .HasForeignKey(e => e.OwnerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Permit
            modelBuilder.Entity<Permit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Vessel)
                      .WithMany(v => v.Permits)
                      .HasForeignKey(e => e.VesselId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Person)
                      .WithMany(p => p.Permits)
                      .HasForeignKey(e => e.PersonId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // LogbookEntry
            modelBuilder.Entity<LogbookEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DurationHours).HasPrecision(10, 2);
                entity.Property(e => e.CatchQuantityKg).HasPrecision(10, 2);
                entity.HasOne(e => e.Vessel)
                      .WithMany(v => v.LogbookEntries)
                      .HasForeignKey(e => e.VesselId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Permit)
                      .WithMany(p => p.LogbookEntries)
                      .HasForeignKey(e => e.PermitId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // FishUnloading
            modelBuilder.Entity<FishUnloading>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuantityKg).HasPrecision(10, 2);
                entity.HasOne(e => e.LogbookEntry)
                      .WithMany(l => l.Unloadings)
                      .HasForeignKey(e => e.LogbookEntryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Inspection
            modelBuilder.Entity<Inspection>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FineAmount).HasPrecision(10, 2);
                entity.HasOne(e => e.Vessel)
                      .WithMany(v => v.Inspections)
                      .HasForeignKey(e => e.VesselId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // AmateurTicket
            modelBuilder.Entity<AmateurTicket>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(10, 2);
            });

            // AmateurCatch
            modelBuilder.Entity<AmateurCatch>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuantityKg).HasPrecision(10, 2);
                entity.HasOne(e => e.AmateurTicket)
                      .WithMany(t => t.Catches)
                      .HasForeignKey(e => e.AmateurTicketId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
