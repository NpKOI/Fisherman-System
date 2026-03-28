using System.Diagnostics;
using FisherMan.ASP.Models;
using Microsoft.AspNetCore.Mvc;

namespace FisherMan.ASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly FisherManContext _context;

        public HomeController(FisherManContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var nextMonth = today.AddMonths(1);
            var startOfYear = new DateTime(today.Year, 1, 1);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Load data into memory first to avoid SQLite decimal Sum issue
            var logbookEntriesThisYear = await _context.LogbookEntries
                .Where(l => l.StartTime >= startOfYear)
                .ToListAsync();

            var dashboard = new DashboardViewModel
            {
                TotalVessels = await _context.Vessels.CountAsync(),
                ActivePermits = await _context.Permits
                    .Where(p => !p.IsRevoked && p.ExpiryDate >= today)
                    .CountAsync(),
                ExpiringPermits = await _context.Permits
                    .Where(p => !p.IsRevoked && p.ExpiryDate >= today && p.ExpiryDate <= nextMonth)
                    .CountAsync(),
                TotalAmateurTickets = await _context.AmateurTickets
                    .Where(t => t.ValidUntil >= today)
                    .CountAsync(),
                InspectionsThisMonth = await _context.Inspections
                    .Where(i => i.InspectionDate >= startOfMonth)
                    .CountAsync(),
                ViolationsThisMonth = await _context.Inspections
                    .Where(i => i.InspectionDate >= startOfMonth && i.ViolationFound)
                    .CountAsync(),
                TotalCatchThisYear = logbookEntriesThisYear.Sum(l => l.CatchQuantityKg)
            };

            // Monthly catch data for chart (using in-memory data)
            dashboard.MonthlyCatch = logbookEntriesThisYear
                .GroupBy(l => l.StartTime.Month)
                .Select(g => new MonthlyStatViewModel
                {
                    Month = g.Key.ToString(),
                    Value = g.Sum(x => x.CatchQuantityKg)
                })
                .OrderBy(x => x.Month)
                .ToList();

            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
