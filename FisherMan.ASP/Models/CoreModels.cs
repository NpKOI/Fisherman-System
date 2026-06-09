using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FisherMan.ASP.Models;

public class Person
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? IdentificationNumber { get; set; }

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [MaxLength(100)]
    public string? BadgeNumber { get; set; }

    [NotMapped]
    public string Name
    {
        get => FullName;
        set => FullName = value;
    }

    public ICollection<Vessel> OwnedVessels { get; set; } = new List<Vessel>();
    public ICollection<Permit> Permits { get; set; } = new List<Permit>();
}

public class Vessel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string InternationalNumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? CallSign { get; set; }

    [Required]
    [MaxLength(50)]
    public string Marking { get; set; } = string.Empty;

    [MaxLength(200)]
    public string CaptainName { get; set; } = string.Empty;

    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Tonnage { get; set; }
    public decimal Draft { get; set; }
    public decimal EnginePower { get; set; }

    [MaxLength(50)]
    public string FuelType { get; set; } = string.Empty;

    public decimal AvgFuelConsumption { get; set; }

    public int OwnerId { get; set; }
    public Person? Owner { get; set; }

    [NotMapped]
    public string OwnerName => Owner?.FullName ?? string.Empty;

    public ICollection<Permit> Permits { get; set; } = new List<Permit>();
    public ICollection<LogbookEntry> LogbookEntries { get; set; } = new List<LogbookEntry>();
    public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
}

public class Permit
{
    public int Id { get; set; }

    public DateTime IssuedDate { get; set; } = DateTime.Today;

    [NotMapped]
    public DateTime IssueDate
    {
        get => IssuedDate;
        set => IssuedDate = value;
    }

    public DateTime ExpiryDate { get; set; } = DateTime.Today.AddYears(1);
    public bool IsRevoked { get; set; }

    [MaxLength(500)]
    public string? RevocationReason { get; set; }

    public DateTime? RevokedDate { get; set; }

    [Required]
    [MaxLength(200)]
    public string CaptainName { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string FishingTools { get; set; } = string.Empty;

    public int VesselId { get; set; }
    public Vessel? Vessel { get; set; }

    [NotMapped]
    public int ShipId
    {
        get => VesselId;
        set => VesselId = value;
    }

    [NotMapped]
    public Vessel? Ship
    {
        get => Vessel;
        set => Vessel = value;
    }

    public int PersonId { get; set; }
    public Person? Person { get; set; }

    public ICollection<LogbookEntry> LogbookEntries { get; set; } = new List<LogbookEntry>();

    [NotMapped]
    public ICollection<FishingTrip> FishingTrips { get; set; } = new List<FishingTrip>();
}

public class LogbookEntry
{
    public int Id { get; set; }

    public int VesselId { get; set; }
    public Vessel? Vessel { get; set; }

    public int PermitId { get; set; }
    public Permit? Permit { get; set; }

    public DateTime StartTime { get; set; } = DateTime.Now;
    public decimal DurationHours { get; set; }
    public decimal CatchQuantityKg { get; set; }

    [MaxLength(200)]
    public string StartLocation { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string FishingTools { get; set; } = string.Empty;

    public ICollection<FishUnloading> Unloadings { get; set; } = new List<FishUnloading>();
}

public class FishUnloading
{
    public int Id { get; set; }

    public int LogbookEntryId { get; set; }
    public LogbookEntry? LogbookEntry { get; set; }

    public DateTime UnloadingDate { get; set; } = DateTime.Today;
    public decimal QuantityKg { get; set; }

    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Receiver { get; set; } = string.Empty;
}

public class Inspection
{
    public int Id { get; set; }

    public int? VesselId { get; set; }
    public Vessel? Vessel { get; set; }

    public int? InspectorId { get; set; }
    public Person? Inspector { get; set; }

    public DateTime InspectionDate { get; set; } = DateTime.Now;
    public InspectionType Type { get; set; }

    [MaxLength(200)]
    public string InspectedObject { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public bool ViolationFound { get; set; }

    [NotMapped]
    public bool HasViolation
    {
        get => ViolationFound;
        set => ViolationFound = value;
    }

    [MaxLength(100)]
    public string? ActNumber { get; set; }

    public decimal? FineAmount { get; set; }
}

public class AmateurTicket
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string FishermanName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string HolderEGN { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; } = DateTime.Today;
    public TicketValidityType ValidityType { get; set; } = TicketValidityType.Yearly;
    public DateTime ValidFrom { get; set; } = DateTime.Today;
    public DateTime ValidUntil { get; set; } = DateTime.Today.AddYears(1);
    public decimal Price { get; set; }
    public bool IsUnder14 { get; set; }
    public bool IsPensioner { get; set; }
    public bool IsDisabled { get; set; }

    [MaxLength(100)]
    public string? TelkDecisionNumber { get; set; }

    [NotMapped]
    public string HolderName
    {
        get => FishermanName;
        set => FishermanName = value;
    }

    [NotMapped]
    public DateTime IssueDate
    {
        get => ValidFrom;
        set => ValidFrom = value;
    }

    [NotMapped]
    public DateTime ExpiryDate
    {
        get => ValidUntil;
        set => ValidUntil = value;
    }

    [NotMapped]
    public string? DisabilityDecisionNumber
    {
        get => TelkDecisionNumber;
        set => TelkDecisionNumber = value;
    }

    [NotMapped]
    public bool IsValid => ValidUntil >= DateTime.Today;

    public ICollection<AmateurCatch> Catches { get; set; } = new List<AmateurCatch>();

    public static decimal CalculatePrice(bool isUnder14, bool isPensioner, bool isDisabled)
    {
        if (isDisabled)
        {
            return 0m;
        }

        if (isUnder14)
        {
            return 5m;
        }

        return isPensioner ? 12m : 25m;
    }
}

public class AmateurCatch
{
    public int Id { get; set; }

    public int AmateurTicketId { get; set; }
    public AmateurTicket? AmateurTicket { get; set; }

    public DateTime CatchDate { get; set; } = DateTime.Today;

    [MaxLength(100)]
    public string FishSpecies { get; set; } = string.Empty;

    [NotMapped]
    public string FishType
    {
        get => FishSpecies;
        set => FishSpecies = value;
    }

    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    public decimal QuantityKg { get; set; }

    [NotMapped]
    public decimal WeightKg
    {
        get => QuantityKg;
        set => QuantityKg = value;
    }
}

public class FishingTrip
{
    public int Id { get; set; }

    public int PermitId { get; set; }
    public Permit? Permit { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime? EndDate { get; set; }

    [NotMapped]
    public decimal? DurationHours =>
        EndDate.HasValue ? (decimal)(EndDate.Value - StartDate).TotalHours : null;

    [MaxLength(200)]
    public string StartLocation { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string FishingTools { get; set; } = string.Empty;

    public decimal TotalCatchKg { get; set; }
    public decimal? FuelConsumed { get; set; }

    public ICollection<FishCatch> Catches { get; set; } = new List<FishCatch>();
}

public class FishCatch
{
    public int Id { get; set; }

    public int FishingTripId { get; set; }
    public FishingTrip? FishingTrip { get; set; }

    [MaxLength(100)]
    public string Species { get; set; } = string.Empty;

    [NotMapped]
    public string FishType
    {
        get => Species;
        set => Species = value;
    }

    public decimal WeightKg { get; set; }
}
