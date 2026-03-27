using FisherMan.ASP.Data;
using FisherMan.ASP.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    // ViewModels for reports
    public class Report1Item
    {
        public Ship Ship { get; set; } = null!;
        public FishingPermit Permit { get; set; } = null!;
        public int DaysUntilExpiry { get; set; }
    }

    public class Report2Item
    {
        public string HolderName { get; set; } = string.Empty;
        public string HolderEGN { get; set; } = string.Empty;
        public decimal TotalCatchKg { get; set; }
        public int Rank { get; set; }
    }

    public class Report3Item
    {
        public Ship Ship { get; set; } = null!;
        public double AvgDurationHours { get; set; }
        public double MinDurationHours { get; set; }
        public double MaxDurationHours { get; set; }
        public double AvgCatchKg { get; set; }
        public double MinCatchKg { get; set; }
        public double MaxCatchKg { get; set; }
        public int TripCount { get; set; }
        public double TotalCatchKgYear { get; set; }
    }

    public class Report4Item
    {
        public Ship Ship { get; set; } = null!;
        public double TotalFuelL { get; set; }
        public double TotalCatchKg { get; set; }
        public double CarbonFootprint { get; set; }
    }

    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reports
        public IActionResult Index()
        {
            return View();
        }

        // Справка 1: Кораби с изтичащо разрешение в следващия 1 месец
        public async Task<IActionResult> Report1()
        {
            var today = DateTime.Today;
            var oneMonthLater = today.AddMonths(1);

            var items = await _context.FishingPermits
                .Include(p => p.Ship)
                .Where(p => !p.IsRevoked && p.ExpiryDate >= today && p.ExpiryDate <= oneMonthLater)
                .OrderBy(p => p.ExpiryDate)
                .Select(p => new Report1Item
                {
                    Ship = p.Ship!,
                    Permit = p,
                    DaysUntilExpiry = (int)(p.ExpiryDate - today).TotalDays
                })
                .ToListAsync();

            return View(items);
        }

        // Справка 2: Класация на любителите с най-голям улов за последната година
        public async Task<IActionResult> Report2()
        {
            var oneYearAgo = DateTime.Today.AddYears(-1);

            var items = await _context.AmateurCatches
                .Include(c => c.Ticket)
                .Where(c => c.CatchDate >= oneYearAgo)
                .GroupBy(c => new { c.Ticket!.HolderName, c.Ticket.HolderEGN })
                .Select(g => new Report2Item
                {
                    HolderName = g.Key.HolderName,
                    HolderEGN = g.Key.HolderEGN,
                    TotalCatchKg = g.Sum(c => c.WeightKg)
                })
                .OrderByDescending(r => r.TotalCatchKg)
                .ToListAsync();

            for (int i = 0; i < items.Count; i++)
                items[i].Rank = i + 1;

            return View(items);
        }

        // Справка 3: Класация на риболовните кораби
        public async Task<IActionResult> Report3()
        {
            var yearStart = new DateTime(DateTime.Today.Year, 1, 1);
            var yearEnd = yearStart.AddYears(1);

            var trips = await _context.FishingTrips
                .Include(t => t.Permit)
                .ThenInclude(p => p != null ? p.Ship : null)
                .Where(t => t.StartDate >= yearStart && t.StartDate < yearEnd && t.EndDate.HasValue)
                .ToListAsync();

            var items = trips
                .GroupBy(t => t.Permit?.Ship)
                .Where(g => g.Key != null)
                .Select(g =>
                {
                    var durations = g.Select(t => (t.EndDate!.Value - t.StartDate).TotalHours).ToList();
                    var catches = g.Select(t => (double)t.TotalCatchKg).ToList();
                    return new Report3Item
                    {
                        Ship = g.Key!,
                        AvgDurationHours = durations.Average(),
                        MinDurationHours = durations.Min(),
                        MaxDurationHours = durations.Max(),
                        AvgCatchKg = catches.Average(),
                        MinCatchKg = catches.Min(),
                        MaxCatchKg = catches.Max(),
                        TripCount = g.Count(),
                        TotalCatchKgYear = catches.Sum()
                    };
                })
                .OrderByDescending(r => r.TotalCatchKgYear)
                .ToList();

            return View(items);
        }

        // Справка 4: Въглероден отпечатък
        public async Task<IActionResult> Report4()
        {
            var yearStart = new DateTime(DateTime.Today.Year, 1, 1);
            var yearEnd = yearStart.AddYears(1);

            // Only ships with active permits
            var activePermitShipIds = await _context.FishingPermits
                .Where(p => !p.IsRevoked && p.ExpiryDate >= DateTime.Today)
                .Select(p => p.ShipId)
                .Distinct()
                .ToListAsync();

            var ships = await _context.Ships
                .Where(s => activePermitShipIds.Contains(s.Id))
                .ToListAsync();

            var trips = await _context.FishingTrips
                .Include(t => t.Permit)
                .Where(t => t.StartDate >= yearStart && t.StartDate < yearEnd && t.EndDate.HasValue)
                .ToListAsync();

            var items = ships
                .Select(ship =>
                {
                    var shipTrips = trips.Where(t => t.Permit?.ShipId == ship.Id).ToList();
                    var totalHours = shipTrips.Sum(t => (t.EndDate!.Value - t.StartDate).TotalHours);
                    var totalFuel = totalHours * (double)ship.FuelConsumptionPerHour;
                    var totalCatch = shipTrips.Sum(t => (double)t.TotalCatchKg);
                    var footprint = totalCatch > 0 ? totalFuel / totalCatch : 0;

                    return new Report4Item
                    {
                        Ship = ship,
                        TotalFuelL = totalFuel,
                        TotalCatchKg = totalCatch,
                        CarbonFootprint = footprint
                    };
                })
                .Where(r => r.TotalCatchKg > 0)
                .OrderBy(r => r.CarbonFootprint)
                .ToList();

            return View(items);
        }
    }
}
