using FisherMan.ASP.Data;
using FisherMan.ASP.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    public class FishingTripsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FishingTripsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FishingTrips
        public async Task<IActionResult> Index(string? search)
        {
            ViewData["Search"] = search;
            var trips = _context.FishingTrips
                .Include(t => t.Permit)
                .ThenInclude(p => p != null ? p.Ship : null)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                trips = trips.Where(t =>
                    t.StartLocation.Contains(search) ||
                    (t.Permit != null && t.Permit.Ship != null && t.Permit.Ship.Marking.Contains(search)));
            }
            return View(await trips.OrderByDescending(t => t.StartDate).ToListAsync());
        }

        // GET: FishingTrips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trip = await _context.FishingTrips
                .Include(t => t.Permit)
                .ThenInclude(p => p != null ? p.Ship : null)
                .Include(t => t.Catches)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trip == null) return NotFound();

            return View(trip);
        }

        // GET: FishingTrips/Create
        public IActionResult Create()
        {
            ViewData["PermitId"] = new SelectList(
                _context.FishingPermits.Include(p => p.Ship).Where(p => !p.IsRevoked && p.ExpiryDate >= DateTime.Today),
                "Id", "CaptainName");
            return View();
        }

        // POST: FishingTrips/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PermitId,StartDate,StartLocation,EndDate,FishingTools,TotalCatchKg,FuelConsumed")] FishingTrip trip)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PermitId"] = new SelectList(
                _context.FishingPermits.Include(p => p.Ship).Where(p => !p.IsRevoked && p.ExpiryDate >= DateTime.Today),
                "Id", "CaptainName", trip.PermitId);
            return View(trip);
        }

        // GET: FishingTrips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trip = await _context.FishingTrips.FindAsync(id);
            if (trip == null) return NotFound();

            ViewData["PermitId"] = new SelectList(
                _context.FishingPermits.Include(p => p.Ship),
                "Id", "CaptainName", trip.PermitId);
            return View(trip);
        }

        // POST: FishingTrips/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PermitId,StartDate,StartLocation,EndDate,FishingTools,TotalCatchKg,FuelConsumed")] FishingTrip trip)
        {
            if (id != trip.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.FishingTrips.Any(e => e.Id == trip.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PermitId"] = new SelectList(
                _context.FishingPermits.Include(p => p.Ship),
                "Id", "CaptainName", trip.PermitId);
            return View(trip);
        }

        // GET: FishingTrips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trip = await _context.FishingTrips
                .Include(t => t.Permit)
                .ThenInclude(p => p != null ? p.Ship : null)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trip == null) return NotFound();

            return View(trip);
        }

        // POST: FishingTrips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trip = await _context.FishingTrips.FindAsync(id);
            if (trip != null) _context.FishingTrips.Remove(trip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
