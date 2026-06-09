namespace FisherMan.ASP.Models;

public class ExpiringPermitViewModel
{
    public string VesselName { get; set; } = string.Empty;
    public string InternationalNumber { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int DaysUntilExpiry { get; set; }
}

public class TopAmateurViewModel
{
    public int Rank { get; set; }
    public string FishermanName { get; set; } = string.Empty;
    public string TicketNumber { get; set; } = string.Empty;
    public decimal TotalCatchKg { get; set; }
    public int TripCount { get; set; }
}

public class VesselStatisticsViewModel
{
    public string VesselName { get; set; } = string.Empty;
    public string InternationalNumber { get; set; } = string.Empty;
    public decimal AvgDuration { get; set; }
    public decimal MinDuration { get; set; }
    public decimal MaxDuration { get; set; }
    public decimal AvgCatch { get; set; }
    public decimal MinCatch { get; set; }
    public decimal MaxCatch { get; set; }
    public int TotalTrips { get; set; }
    public decimal TotalCatchKg { get; set; }
}

public class CarbonFootprintViewModel
{
    public string VesselName { get; set; } = string.Empty;
    public string InternationalNumber { get; set; } = string.Empty;
    public decimal TotalFuelConsumed { get; set; }
    public decimal TotalCatchKg { get; set; }
    public decimal CarbonPerKg { get; set; }
    public string EfficiencyRating { get; set; } = string.Empty;
}
