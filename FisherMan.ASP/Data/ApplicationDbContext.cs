using FisherMan.ASP.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ship> Ships { get; set; }
        public DbSet<FishingPermit> FishingPermits { get; set; }
        public DbSet<FishingTrip> FishingTrips { get; set; }
        public DbSet<TripCatch> TripCatches { get; set; }
        public DbSet<FishingTicket> FishingTickets { get; set; }
        public DbSet<AmateurCatch> AmateurCatches { get; set; }
        public DbSet<Inspector> Inspectors { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
    }
}
