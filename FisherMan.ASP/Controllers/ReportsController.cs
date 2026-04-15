using FisherMan.ASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    public class ReportsController : Controller
    {
        private readonly FishermanContext _context;

        public ReportsController(FishermanContext context)
        {
            _context = context;
        }

        // Reports Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // Report 1: Expiring Permits (next 1 month)
        public async Task<IActionResult> ExpiringPermits()
        {
            var today = DateTime.Today;
            var nextMonth = today.AddMonths(1);

            var results = await _context.Permits
                .Include(p => p.Vessel).ThenInclude(v => v!.Owner)
                .Include(p => p.Person)
                .Where(p => !p.IsRevoked && p.ExpiryDate >= today && p.ExpiryDate <= nextMonth)
                .Select(p => new ExpiringPermitViewModel
                {
                    VesselName = p.Vessel!.Marking,
                    InternationalNumber = p.Vessel.InternationalNumber,
                    OwnerName = p.Person != null ? p.Person.FullName : (p.Vessel.Owner != null ? p.Vessel.Owner.FullName : "Unknown"),
                    ExpiryDate = p.ExpiryDate,
                    DaysUntilExpiry = (p.ExpiryDate - today).Days
                })
                .OrderBy(x => x.DaysUntilExpiry)
                .ToListAsync();

            return View(results);
        }

        // Report 2: Top Amateur Fishermen (last 1 year)
        public async Task<IActionResult> TopAmateurs()
        {
            var oneYearAgo = DateTime.Today.AddYears(-1);

            // Load into memory first to avoid SQLite decimal Sum issue
            var catches = await _context.AmateurCatches
                .Include(c => c.AmateurTicket)
                .Where(c => c.CatchDate >= oneYearAgo)
                .ToListAsync();

            var results = catches
                .GroupBy(c => new { c.AmateurTicketId, c.AmateurTicket!.FishermanName })
                .Select(g => new TopAmateurViewModel
                {
                    FishermanName = g.Key.FishermanName,
                    TicketNumber = g.Key.AmateurTicketId.ToString(),
                    TotalCatchKg = g.Sum(x => x.QuantityKg),
                    TripCount = g.Count()
                })
                .OrderByDescending(x => x.TotalCatchKg)
                .ToList();

            // Add ranking
            for (int i = 0; i < results.Count; i++)
            {
                results[i].Rank = i + 1;
            }

            return View(results);
        }

        // Report 3: Vessel Statistics
        public async Task<IActionResult> VesselStatistics()
        {
            var startOfYear = new DateTime(DateTime.Today.Year, 1, 1);

            // Load into memory first to avoid SQLite decimal aggregation issues
            var entries = await _context.LogbookEntries
                .Include(l => l.Vessel)
                .Where(l => l.StartTime >= startOfYear)
                .ToListAsync();

            var results = entries
                .GroupBy(l => new { l.VesselId, l.Vessel!.Marking, l.Vessel.InternationalNumber })
                .Select(g => new VesselStatisticsViewModel
                {
                    VesselName = g.Key.Marking,
                    InternationalNumber = g.Key.InternationalNumber,
                    AvgDuration = g.Average(x => x.DurationHours),
                    MinDuration = g.Min(x => x.DurationHours),
                    MaxDuration = g.Max(x => x.DurationHours),
                    AvgCatch = g.Average(x => x.CatchQuantityKg),
                    MinCatch = g.Min(x => x.CatchQuantityKg),
                    MaxCatch = g.Max(x => x.CatchQuantityKg),
                    TotalTrips = g.Count(),
                    TotalCatchKg = g.Sum(x => x.CatchQuantityKg)
                })
                .OrderByDescending(x => x.TotalCatchKg)
                .ToList();

            return View(results);
        }

        // Report 4: Carbon Footprint
        public async Task<IActionResult> CarbonFootprint()
        {
            var today = DateTime.Today;
            var startOfYear = new DateTime(today.Year, 1, 1);

            // Get vessels with valid (non-expired) permits
            var vesselsWithValidPermits = await _context.Permits
                .Where(p => !p.IsRevoked && p.ExpiryDate >= today)
                .Select(p => p.VesselId)
                .Distinct()
                .ToListAsync();

            // Load into memory first to avoid SQLite decimal aggregation issues
            var entries = await _context.LogbookEntries
                .Include(l => l.Vessel)
                .Where(l => l.StartTime >= startOfYear && vesselsWithValidPermits.Contains(l.VesselId))
                .ToListAsync();

            var results = entries
                .GroupBy(l => new {
                    l.VesselId,
                    l.Vessel!.Marking,
                    l.Vessel.InternationalNumber,
                    l.Vessel.AvgFuelConsumption
                })
                .Select(g => new
                {
                    VesselName = g.Key.Marking,
                    InternationalNumber = g.Key.InternationalNumber,
                    AvgFuelConsumption = g.Key.AvgFuelConsumption,
                    TotalHours = g.Sum(x => x.DurationHours),
                    TotalCatchKg = g.Sum(x => x.CatchQuantityKg)
                })
                .ToList();

            var carbonResults = results
                .Where(r => r.TotalCatchKg > 0)
                .Select(r => {
                    var totalFuel = r.AvgFuelConsumption * r.TotalHours;
                    var carbonPerKg = totalFuel / r.TotalCatchKg;
                    return new CarbonFootprintViewModel
                    {
                        VesselName = r.VesselName,
                        InternationalNumber = r.InternationalNumber,
                        TotalFuelConsumed = totalFuel,
                        TotalCatchKg = r.TotalCatchKg,
                        CarbonPerKg = carbonPerKg,
                        EfficiencyRating = carbonPerKg switch
                        {
                            < 1 => "Excellent",
                            < 2 => "Good",
                            < 3 => "Average",
                            _ => "Poor"
                        }
                    };
                })
                .OrderBy(x => x.CarbonPerKg)
                .ToList();

            return View(carbonResults);
        }

        // Extra Feature: Quick TELK Search for Inspectors
        [HttpGet]
        public async Task<IActionResult> SearchTelk(string telkNumber)
        {
            if (string.IsNullOrEmpty(telkNumber))
                return Json(new { found = false });

            var ticket = await _context.AmateurTickets
                .Where(t => t.TelkDecisionNumber != null && t.TelkDecisionNumber.Contains(telkNumber))
                .Select(t => new
                {
                    t.FishermanName,
                    t.TelkDecisionNumber,
                    t.ValidFrom,
                    t.ValidUntil,
                    IsValid = t.ValidUntil >= DateTime.Today
                })
                .FirstOrDefaultAsync();

            if (ticket == null)
                return Json(new { found = false });

            return Json(new { found = true, ticket });
        }
    }
}
